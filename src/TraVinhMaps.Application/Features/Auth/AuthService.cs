// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TraVinhMaps.Application.External;
using TraVinhMaps.Application.Features.Auth.Interface;
using TraVinhMaps.Application.Features.Auth.Models;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Auth;

public class AuthService : IAuthServices
{
    private readonly ISpeedSmsService _speedSmsService;
    private IUnitOfWork _unitOfWork;
    public AuthService(ISpeedSmsService speedSmsService, IUnitOfWork unitOfWork)
    {
        _speedSmsService = speedSmsService;
        _unitOfWork = unitOfWork;
    }
    public Task<AuthResponse> AdminLogin(string email, string password, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ChangePassword(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Logout(string userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<OtpResponse> RequestMobileLoginOtp(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var phone = new string[] { phoneNumber };
        await _speedSmsService.sendSms(phone, "your otp is 123456", 2, "");
        return null;
    }
    public async Task<string> RequestMobileRegistrationOtp(string phoneNumber, CancellationToken cancellationToken = default)
    {

        var session = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var phoneExits = await _unitOfWork.GetRepository<User>().GetAsyns(x => x.PhoneNumber == phoneNumber, cancellationToken);
            if (phoneExits != null)
            {
                return "Phone number already exists";
            }

            var otp = GenerateOtp();
            var commitOtp = _unitOfWork.GetRepository<Otp>().AddAsync(new Otp
            {
                Id = Guid.NewGuid().ToString(),
                OtpCode = otp,
                PhoneNumber = phoneNumber,
                CreatedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            }, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(session, cancellationToken);
            return otp;
        }
        catch (Exception)
        {
            await _unitOfWork.AbortTransactionAsync(session, cancellationToken);
            throw;
        }
    }


    public Task<AuthResponse> VerifyMobileLogin(string phoneNumber, string otp, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponse> VerifyMobileRegistration(string phoneNumber, string password, string otp, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    private string GenerateOtp()
    {
        Random random = new Random();
        int otp = random.Next(100000, 999999);
        return otp.ToString();
    }
}
