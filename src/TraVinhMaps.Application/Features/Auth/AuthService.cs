// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Common.Extensions;
using TraVinhMaps.Application.External;
using TraVinhMaps.Application.Features.Auth.Interface;
using TraVinhMaps.Application.Features.Auth.Models;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Auth;

public class AuthService : IAuthServices
{
    private readonly ISpeedSmsService _speedSmsService;
    private readonly IRepository<Otp> _otpRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly ICacheService _cacheService;
    private readonly IRepository<UserSession> _sessionRepository;
    private readonly IEmailSender _emailSender;
    private const string CacheKey = "otp_context:";

    public AuthService(ISpeedSmsService speedSmsService, IRepository<Otp> repository, IRepository<User> userRepository, IRepository<Role> roleRepository, IRepository<UserSession> sessionRepository, ICacheService cacheService, IEmailSender emailSender)
    {
        _speedSmsService = speedSmsService;
        _otpRepository = repository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _sessionRepository = sessionRepository;
        _cacheService = cacheService;
        _emailSender = emailSender;
    }

    public async Task<string> AuthenWithPhoneNumber(string phoneNumber, CancellationToken cancellationToken)
    {
        var otp = GenarateOtpExtension.GenerateOtp();

        // send sms
        var message = $"Your OTP in TRAVINHGO is {otp}";
        await _speedSmsService.SendSMS(phoneNumber, message);

        // save OTP to db

        var otpEntity = new Otp
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            ActualIdentifier = phoneNumber,
            IdentifierType = "phone-number",
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            AttemptCount = 0,
            HashedOtpCode = HashingExtension.HashOtp(otp)
        };
        await _otpRepository.AddAsync(otpEntity, cancellationToken);

        var contextId = Guid.NewGuid().ToString();
        // save to cache
        var key = CacheKey + contextId;
        await _cacheService.SetData(key, otpEntity.Id);

        // return
        return contextId;
    }

    public async Task<string> AuthenWithEmail(string email, CancellationToken cancellationToken)
    {
        var otp = GenarateOtpExtension.GenerateOtp();

        // send otp via email
        await _emailSender.SendEmailAsync(email, "OTP Verification For TraVinhGo", otp, cancellationToken);

        // save OTP to db

        var otpEntity = new Otp
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            ActualIdentifier = email,
            IdentifierType = "email",
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            AttemptCount = 0,
            HashedOtpCode = HashingExtension.HashOtp(otp)
        };
        await _otpRepository.AddAsync(otpEntity, cancellationToken);

        var contextId = Guid.NewGuid().ToString();
        // save to cache
        var key = CacheKey + contextId;
        await _cacheService.SetData(key, otpEntity.Id);

        // return
        return contextId;
    }

    public async Task<AuthResponse> VerifyOtp(string identifier, string otp, string? device, string? ipAddress, CancellationToken cancellationToken)
    {
        var actualIdentifier = await _cacheService.GetData<string>(CacheKey + identifier);
        if (actualIdentifier == null)
        {
            return null;
        }

        var otpEntity = await _otpRepository.GetAsyns(x => x.Id == actualIdentifier && x.IsUsed == false, cancellationToken);
        if (otpEntity == null)
        {
            throw new NotFoundException("otp is not found");
        }

        // Check if OTP is expired
        if (otpEntity.ExpiredAt < DateTime.UtcNow)
        {
            await _cacheService.RemoveData(CacheKey + identifier);
            otpEntity.IsUsed = true;
            await _otpRepository.UpdateAsync(otpEntity, cancellationToken);
            throw new BadRequestException("OTP is expired");
        }

        // Check if max attempts exceeded
        if (otpEntity.AttemptCount > 5)
        {
            await _cacheService.RemoveData(CacheKey + identifier);
            otpEntity.IsUsed = true;
            await _otpRepository.UpdateAsync(otpEntity, cancellationToken);
            throw new BadRequestException("OTP is expired");
        }

        // Verify OTP hash
        var hashingOtp = HashingExtension.HashOtp(otp);
        if (otpEntity.HashedOtpCode != hashingOtp)
        {
            otpEntity.AttemptCount++;
            otpEntity.LastAttemptAt = DateTime.UtcNow;
            await _otpRepository.UpdateAsync(otpEntity, cancellationToken);
            throw new BadRequestException("Invalid OTP");
        }

        // Mark OTP as used and remove from cache
        otpEntity.IsUsed = true;
        await _otpRepository.UpdateAsync(otpEntity, cancellationToken);
        await _cacheService.RemoveData(CacheKey + identifier);

        // Authenticate or create user based on identifier type
        string userId;
        var type = otpEntity.IdentifierType;

        if (type == "phone-number")
        {
            var user = await _userRepository.GetAsyns(x => x.PhoneNumber == actualIdentifier, cancellationToken);
            if (user == null)
            {
                var role = await _roleRepository.GetAsyns(x => x.RoleName == "user", cancellationToken);
                var newUser = new User
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    PhoneNumber = actualIdentifier,
                    CreatedAt = DateTime.UtcNow,
                    RoleId = role.Id,
                    Status = true,
                };
                await _userRepository.AddAsync(newUser, cancellationToken);
                userId = newUser.Id;
            }
            else
            {
                userId = user.Id;
            }
        }
        else
        {
            var user = await _userRepository.GetAsyns(x => x.Email == actualIdentifier, cancellationToken);
            if (user == null)
            {
                var role = await _roleRepository.GetAsyns(x => x.RoleName == "user", cancellationToken);
                var newUser = new User
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    Email = actualIdentifier,
                    CreatedAt = DateTime.UtcNow,
                    RoleId = role.Id,
                    Status = true,
                };
                await _userRepository.AddAsync(newUser, cancellationToken);
                userId = newUser.Id;
            }
            else
            {
                userId = user.Id;
            }
        }

        // Generate session tokens
        var sessionID = Guid.NewGuid().ToString();
        var refreshToken = Guid.NewGuid().ToString();

        // Create new session
        var session = new UserSession
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.AddDays(1),
            SessionId = HashingTokenExtension.HashToken(sessionID),
            RefreshToken = HashingTokenExtension.HashToken(refreshToken),
            RefreshTokenExpireAt = DateTime.UtcNow.AddDays(7),
            IsActive = true,
            DeviceInfo = device,
            IpAddress = ipAddress
        };

        await _sessionRepository.AddAsync(session, cancellationToken);
        // Enforce session limit with 3 devices
        await EnforceSessionLimitAsync(userId, session, cancellationToken);
        // Return response
        return new AuthResponse
        {
            SessionId = sessionID,
            RefreshToken = refreshToken
        };
    }

    public async Task Logout(string sessionId, CancellationToken cancellationToken = default)
    {
        var hashedSessionId = HashingTokenExtension.HashToken(sessionId);

        var session = await _sessionRepository.GetAsyns(
         s => s.SessionId == hashedSessionId &&
              s.IsActive == true,
         cancellationToken);

        if (session == null)
        {
            return;
        }
        session.IsActive = false;
        await _sessionRepository.UpdateAsync(session, cancellationToken);
    }

    public async Task EnforceSessionLimitAsync(string userId, UserSession newSession, CancellationToken cancellationToken = default)
    {
        var activeSessions = await _sessionRepository.ListAsync(
     s => s.UserId == userId && s.IsActive == true,
     cancellationToken);

        if (activeSessions.Count() <= 3)
        {
            return;
        }

        var oldestSession = activeSessions
          .OrderBy(s => s.CreatedAt)
          .FirstOrDefault();

        if (oldestSession != null)
        {
            oldestSession.IsActive = false;
            await _sessionRepository.UpdateAsync(oldestSession, cancellationToken);
        }
    }

    public async Task<List<SessionUserResponse>> GetAllSessionUser(string userId, CancellationToken cancellationToken = default)
    {
        var activeSession = await _sessionRepository.ListAsync(x => x.UserId == userId && x.IsActive == true, cancellationToken);
        var result = new List<SessionUserResponse>();
        foreach (var session in activeSession)
        {
            result.Add(new SessionUserResponse
            {
                UserId = session.UserId,
                DeviceInfo = session.DeviceInfo,
                IpAddress = session.IpAddress,
            });
        }
        return result;
    }
}
