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
    private readonly IBaseRepository<Otp> _otpRepository;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly ICacheService _cacheService;
    private readonly IBaseRepository<UserSession> _sessionRepository;
    private readonly IEmailSender _emailSender;
    private const string CacheKey = "otp_context:";
    public AuthService(ISpeedSmsService speedSmsService, IBaseRepository<Otp> repository, IBaseRepository<User> userRepository, IBaseRepository<Role> roleRepository, IBaseRepository<UserSession> sessionRepository, ICacheService cacheService, IEmailSender emailSender)
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
        if (phoneNumber == null)
        {
            throw new ArgumentNullException("phoneNumber is null");
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^(\+\d{1,3}[- ]?)?\d{10,15}$"))
        {
            throw new FormatException("Invalid phone number format. Must be a valid phone number.");
        }

        var exits = await _userRepository.GetAsyns(x => x.PhoneNumber == phoneNumber, cancellationToken);
        if (exits != null)
        {
            if (!exits.Status || exits.IsForbidden)
            {
                throw new BadRequestException("This account is locked");
            }
            var currentRole = await _roleRepository.GetByIdAsync(exits.RoleId, cancellationToken);
            if (currentRole == null || (currentRole.RoleName != "user"))
            {
                throw new BadRequestException("this account not admin");
            }
        }

        var otp = GenarateOtpExtension.GenerateOtp();

        // send sms
        var message = $"Your OTP in TRAVINHGO is {otp}";
        await _speedSmsService.SendSMS(phoneNumber, message);

        // save OTP to db

        var otpEntity = new Otp
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Identifier = phoneNumber,
            IdentifierType = "phone-number",
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            AttemptCount = 0,
            HashedOtpCode = HashingExtension.HashWithSHA256(otp)
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
        var user = await _userRepository.GetAsyns(x => x.Email == email, cancellationToken);
        if (user != null)
        {
            if (!user.Status || user.IsForbidden)
            {
                throw new BadRequestException("This account is locked");
            }

            var currentRole = _roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
            if (currentRole == null || (currentRole.Result.RoleName != "user"))
            {
                throw new BadRequestException("this account not admin");
            }
        }
        var otp = GenarateOtpExtension.GenerateOtp();

        // send otp via email
        await _emailSender.SendEmailAsync(email, "OTP Verification For TraVinhGo", otp, cancellationToken);

        // save OTP to db

        var otpEntity = new Otp
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Identifier = email,
            IdentifierType = "email",
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            AttemptCount = 0,
            HashedOtpCode = HashingExtension.HashWithSHA256(otp)
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
        var hashingOtp = HashingExtension.HashWithSHA256(otp);
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

        // Authenticate or create session based on identifier type
        string userId;
        var type = otpEntity.IdentifierType;

        if (type == "phone-number")
        {
            var user = await _userRepository.GetAsyns(x => x.PhoneNumber == otpEntity.Identifier, cancellationToken);
            if (user == null)
            {
                var role = await _roleRepository.GetAsyns(x => x.RoleName == "user", cancellationToken);
                var newUser = new User
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    PhoneNumber = otpEntity.Identifier, // phoneNumber
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
            var user = await _userRepository.GetAsyns(x => x.Email == otpEntity.Identifier, cancellationToken);
            if (user == null)
            {
                var role = await _roleRepository.GetAsyns(x => x.RoleName == "user", cancellationToken);
                var newUser = new User
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    Email = otpEntity.Identifier, // email
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
        var curentUser = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (curentUser == null)
        {
            throw new NotFoundException("User not found");
        }
        var currentRole = await _roleRepository.GetByIdAsync(curentUser.RoleId, cancellationToken);
        // Return response
        return new AuthResponse
        {
            SessionId = sessionID,
            RefreshToken = refreshToken,
            Role = currentRole.RoleName // Include role in the response
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
        //session.RefreshToken = null;
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

    public async Task<string> RefreshOtp(string item, CancellationToken cancellationToken = default)
    {
        // Use regex to determine if the input is an email or phone number
        bool isEmail = System.Text.RegularExpressions.Regex.IsMatch(item, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        bool isPhoneNumber = System.Text.RegularExpressions.Regex.IsMatch(item, @"^(\+\d{1,3}[- ]?)?\d{10,15}$");

        if (!isEmail && !isPhoneNumber)
        {
            throw new BadRequestException("Invalid input format. Must be a valid email or phone number.");
        }

        // Check all the OTP already exists in the database
        // and mark them as used
        var existingOtp = await _otpRepository.ListAsync(x => x.Identifier == item && x.IsUsed == false, cancellationToken);
        foreach (var otpItem in existingOtp)
        {
            if (otpItem.ExpiredAt > DateTime.UtcNow)
            {
                otpItem.IsUsed = true;
                await _otpRepository.UpdateAsync(otpItem, cancellationToken);
            }
        }
        // Generate a new OTP
        var otp = GenarateOtpExtension.GenerateOtp();

        // Create a new OTP entity
        var otpEntity = new Otp
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Identifier = item,
            IdentifierType = isEmail ? "email" : "phone-number",
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            AttemptCount = 0,
            HashedOtpCode = HashingExtension.HashWithSHA256(otp)
        };

        // Save OTP to database
        await _otpRepository.AddAsync(otpEntity, cancellationToken);

        // Generate context ID and save to cache
        var contextId = Guid.NewGuid().ToString();
        var key = CacheKey + contextId;
        await _cacheService.SetData(key, otpEntity.Id);

        // Send OTP based on identifier type
        if (isEmail)
        {
            await _emailSender.SendEmailAsync(item, "OTP Verification For TraVinhGo", otp, cancellationToken);
        }
        else // phone number
        {
            var message = $"Your OTP in TRAVINHGO is {otp}";
            await _speedSmsService.SendSMS(item, message);
        }

        return contextId;
    }

    public async Task<string> AuthenAdminWithCredentials(string identifier, string password, CancellationToken cancellationToken = default)
    {
        if (identifier == null || password == null)
        {
            throw new BadRequestException("Identifier and password cannot be null");
        }
        var user = await _userRepository.GetAsyns(x => x.Email == identifier || x.PhoneNumber == identifier, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var hashPass = HashingExtension.HashWithSHA256(password);
        if (user.Password != hashPass)
        {
            throw new BadRequestException("Invalid password");
        }

        if (user.Status == false || user.IsForbidden)
        {
            throw new BadRequestException("This account is locked");
        }

        // check with the role admin
        var role = await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
        if (role == null || (role.RoleName != "admin" && role.RoleName != "super-admin"))
        {
            throw new BadRequestException("this account not admin");
        }

        // Generate OTP
        var otp = GenarateOtpExtension.GenerateOtp();

        bool isEmail = System.Text.RegularExpressions.Regex.IsMatch(identifier, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        // the indentifier is not email
        if (!isEmail)
        {
            var message = $"Your OTP in TRAVINHGO is {otp}";
            await _speedSmsService.SendSMS(user.PhoneNumber, message);
        }
        else
        {
            await _emailSender.SendEmailAsync(user.Email, "OTP Verification For TraVinhGo", otp, cancellationToken);
        }
        var otpEntity = new Otp
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Identifier = isEmail ? user.Email : user.PhoneNumber,
            IdentifierType = isEmail ? "email" : "phone-number",
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            AttemptCount = 0,
            HashedOtpCode = HashingExtension.HashWithSHA256(otp)
        };
        await _otpRepository.AddAsync(otpEntity, cancellationToken);

        var contextId = Guid.NewGuid().ToString();
        // save to cache
        var key = CacheKey + contextId;
        await _cacheService.SetData(key, otpEntity.Id);

        // return
        return contextId;
    }

    public async Task<AuthResponse> VerifyOtpAdmin(string identifier, string otp, string? device, string? ipAddress, CancellationToken cancellationToken = default)
    {

        var actualIdentifier = await _cacheService.GetData<string>(CacheKey + identifier);
        if (actualIdentifier == null)
        {
            throw new NotFoundException("otp is not found");
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
        var hashingOtp = HashingExtension.HashWithSHA256(otp);
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

        // Authenticate or create session based on identifier type
        var user = await _userRepository.GetAsyns(x => x.Email == otpEntity.Identifier || x.PhoneNumber == otpEntity.Identifier, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        var sessionID = Guid.NewGuid().ToString();
        var refreshToken = Guid.NewGuid().ToString();

        // Create new session
        var session = new UserSession
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            UserId = user.Id,
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
        await EnforceSessionLimitAsync(user.Id, session, cancellationToken);

        var role = await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
        // Return response
        return new AuthResponse
        {
            SessionId = sessionID,
            RefreshToken = refreshToken,
            Role = role.RoleName // Include role in the response
        };
    }

    public async Task<string> ForgetPassword(string identifier)
    {

        bool isEmail = System.Text.RegularExpressions.Regex.IsMatch(identifier, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        bool isPhoneNumber = System.Text.RegularExpressions.Regex.IsMatch(identifier, @"^(\+\d{1,3}[- ]?)?\d{10,15}$");

        if (!isEmail && !isPhoneNumber)
        {
            throw new BadRequestException("Invalid input format. Must be a valid email or phone number.");
        }

        if (isEmail)
        {
            var user = await _userRepository.GetAsyns(x => x.Email == identifier);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            if (user.IsForbidden)
            {
                throw new BadRequestException("This account is locked");
            }

            if (user.Status == false)
            {
                throw new BadRequestException("This account is locked");
            }

            var otp = GenarateOtpExtension.GenerateOtp();
            await _emailSender.SendEmailAsync(user.Email, "OTP Verification For TraVinhGo", otp);
            var otpEntity = new Otp
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                Identifier = user.Email,
                IdentifierType = "email",
                CreatedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                AttemptCount = 0,
                HashedOtpCode = HashingExtension.HashWithSHA256(otp)
            };

            await _otpRepository.AddAsync(otpEntity);
            var contextId = Guid.NewGuid().ToString();
            // save to cache
            var key = CacheKey + contextId;
            await _cacheService.SetData(key, otpEntity.Id);
            return contextId;
        }
        else
        {
            var user = await _userRepository.GetAsyns(x => x.PhoneNumber == identifier);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            if (user.IsForbidden)
            {
                throw new BadRequestException("This account is locked");
            }
            if (user.Status == false)
            {
                throw new BadRequestException("This account is locked");
            }
            var otp = GenarateOtpExtension.GenerateOtp();
            var message = $"Your OTP in TRAVINHGO is {otp}";
            await _speedSmsService.SendSMS(user.PhoneNumber, message);
            var otpEntity = new Otp
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                Identifier = user.PhoneNumber,
                IdentifierType = "phone-number",
                CreatedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                AttemptCount = 0,
                HashedOtpCode = HashingExtension.HashWithSHA256(otp)
            };
            await _otpRepository.AddAsync(otpEntity);
            var contextId = Guid.NewGuid().ToString();
            // save to cache
            var key = CacheKey + contextId;
            await _cacheService.SetData(key, otpEntity.Id);
            return contextId;
        }
    }

    public async Task<bool> ResetPassword(string identifier, string newPassword)
    {
        if (identifier == null || newPassword == null)
        {
            throw new BadRequestException("idetifier and password must not null");
        }
        var user = await _userRepository.GetAsyns(x => x.Email == identifier || x.PhoneNumber == identifier);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        try
        {
            user.Password = HashingExtension.HashWithSHA256(newPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }
        catch (Exception ex)
        {
            throw new BadRequestException("Fail to update new password");
        }
    }

    public async Task<bool> VerifyOtpForResetPassword(string identifier, string otp, CancellationToken cancellationToken = default)
    {
        var actualIdentifier = await _cacheService.GetData<string>(CacheKey + identifier);
        if (actualIdentifier == null)
        {
            throw new NotFoundException("otp is not found");
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
            return false;
        }

        // Check if max attempts exceeded
        if (otpEntity.AttemptCount > 5)
        {
            await _cacheService.RemoveData(CacheKey + identifier);
            otpEntity.IsUsed = true;
            await _otpRepository.UpdateAsync(otpEntity, cancellationToken);
            return false;
        }

        // Verify OTP hash
        var hashingOtp = HashingExtension.HashWithSHA256(otp);
        if (otpEntity.HashedOtpCode != hashingOtp)
        {
            otpEntity.AttemptCount++;
            otpEntity.LastAttemptAt = DateTime.UtcNow;
            await _otpRepository.UpdateAsync(otpEntity, cancellationToken);
            return false;
        }

        // Mark OTP as used and remove from cache
        otpEntity.IsUsed = true;
        await _otpRepository.UpdateAsync(otpEntity, cancellationToken);
        await _cacheService.RemoveData(CacheKey + identifier);
        return true;
    }

    public async Task<string> AuthenWithEmailAdmin(string email, CancellationToken cancellationToken = default)
    {
        if (email == null)
        {
            throw new BadRequestException("Identifier cannot be null");
        }
        var user = await _userRepository.GetAsyns(x => x.Email == email, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var currentRole = user.RoleId;

        var role = await _roleRepository.GetByIdAsync(currentRole, cancellationToken);

        if (role == null || (role.RoleName != "admin" && role.RoleName != "super-admin"))
        {
            throw new BadRequestException("this account not admin");
        }

        if (!user.Status || user.IsForbidden)
        {
            throw new BadRequestException("This account is locked");
        }
        var otp = GenarateOtpExtension.GenerateOtp();
        await _emailSender.SendEmailAsync(user.Email, "OTP Verification For TraVinhGo", otp, cancellationToken);
        var otpEntity = new Otp
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Identifier = user.Email,
            IdentifierType = "email",
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            AttemptCount = 0,
            HashedOtpCode = HashingExtension.HashWithSHA256(otp)
        };
        await _otpRepository.AddAsync(otpEntity, cancellationToken);

        var contextId = Guid.NewGuid().ToString();
        // save to cache
        var key = CacheKey + contextId;
        await _cacheService.SetData(key, otpEntity.Id);

        // return
        return contextId;
    }

    public async Task<AuthResponse> RefreshToken(string refreshToken, CancellationToken cancellationToken = default)
    {
        var hashedRefresh = HashingTokenExtension.HashToken(refreshToken);
        var session = await _sessionRepository.GetAsyns(x => x.RefreshToken == hashedRefresh && !x.IsActive, cancellationToken);
        if (session == null)
        {
            throw new UnauthorizedException("Invalid session or refresh token");
        }
        if (session.RefreshTokenExpireAt < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh token is expired");
        }
        // create new session
        var newSessionId = Guid.NewGuid().ToString();
        var newRefreshToken = Guid.NewGuid().ToString();
        var newSession = new UserSession
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            UserId = session.UserId,
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.AddDays(1),
            SessionId = HashingTokenExtension.HashToken(newSessionId),
            RefreshToken = HashingTokenExtension.HashToken(newRefreshToken),
            RefreshTokenExpireAt = DateTime.UtcNow.AddDays(7),
            IsActive = true,
            DeviceInfo = "",
            IpAddress = ""
        };
        await _sessionRepository.AddAsync(newSession, cancellationToken);
        // Enforce session limit with 3 devices
        await EnforceSessionLimitAsync(session.UserId, newSession, cancellationToken);
        var roleName = await _roleRepository.GetByIdAsync(session.UserId, cancellationToken);
        // Return new session and refresh token
        return new AuthResponse
        {
            SessionId = newSessionId,
            RefreshToken = newRefreshToken,
            Role = roleName.RoleName // Include role in the response
        };
    }
}
