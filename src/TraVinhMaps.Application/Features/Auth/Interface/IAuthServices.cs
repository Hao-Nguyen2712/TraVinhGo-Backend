// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TraVinhMaps.Application.Features.Auth.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Auth.Interface;

public interface IAuthServices
{
    // Send OTP to the user's phone number
    // authen request with phone number
    Task<string> AuthenWithPhoneNumber(string phoneNumber, CancellationToken cancellationToken = default);
    // Send OTP to the user's email
    // authen request with email
    Task<string> AuthenWithEmail(string email, CancellationToken cancellationToken = default);
    // Verify request the OTP
    // authen request with otp
    Task<AuthResponse> VerifyOtp(string identifier, string otp, string? device, string? ipAddress, CancellationToken cancellationToken = default);

    // Logout the sesion
    Task Logout(string sessionId, CancellationToken cancellationToken = default);

    // Control the sesion in differrent Device
    Task EnforceSessionLimitAsync(string userId, UserSession newSession, CancellationToken cancellationToken = default);
    // Get the session from the user
    Task<List<SessionUserResponse>> GetAllSessionUser(string userId, CancellationToken cancellationToken = default);

    Task<string> RefreshOtp(string item, CancellationToken cancellationToken = default);

}
