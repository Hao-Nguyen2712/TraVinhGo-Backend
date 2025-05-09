// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TraVinhMaps.Application.Features.Auth.Models;

namespace TraVinhMaps.Application.Features.Auth.Interface;

public interface IAuthServices
{
    // Mobile user authentication (two-step with OTP)
    Task<OtpResponse> RequestMobileLoginOtp(string phoneNumber, CancellationToken cancellationToken = default);
    Task<AuthResponse> VerifyMobileLogin(string phoneNumber, string otp, CancellationToken cancellationToken = default);

    // Mobile user registration (two-step with OTP)
    Task<string> RequestMobileRegistrationOtp(string phoneNumber, CancellationToken cancellationToken = default);
    Task<AuthResponse> VerifyMobileRegistration(string phoneNumber, string password, string otp, CancellationToken cancellationToken = default);

    // Admin authentication (email + password)
    Task<AuthResponse> AdminLogin(string email, string password, CancellationToken cancellationToken = default);

    // Common operations
    Task<bool> ChangePassword(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task<bool> Logout(string userId, CancellationToken cancellationToken = default);
}
