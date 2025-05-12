// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Features.Auth.Interface;

namespace TraVinhMaps.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthServices _authServices;

    public AuthController(IAuthServices authServices)
    {
        _authServices = authServices;
    }

    #region Request Authen for User endpoint

    [HttpPost("request-email-authen")]
    public async Task<IActionResult> AuthenWithEmail(string email)
    {
        var result = await _authServices.AuthenWithEmail(email);
        return this.ApiOk(result, "Email authentication requested successfully");
    }

    [HttpPost("request-phonenumber-authen")]
    public async Task<IActionResult> AuthenWithPhoneNumber(string phoneNumber)
    {
        var result = await _authServices.AuthenWithPhoneNumber(phoneNumber);
        return this.ApiOk(result, "Phone number authentication requested successfully");
    }

    [HttpPost("confirm-otp-authen")]
    public async Task<IActionResult> Confirm(string otp)
    {
        var id = Request.Headers["id"].ToString();
        if (string.IsNullOrEmpty(id))
        {
            return this.ApiError("ID is required", HttpStatusCode.Unauthorized);
        }
        var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault()
                 ?? Request.HttpContext.Connection.RemoteIpAddress?.ToString();

        string? device = Request.Headers["device"].ToString();
        var result = await _authServices.VerifyOtp(id, otp, device, ip);
        if (result == null)
        {
            return this.ApiError("Invalid OTP", HttpStatusCode.Unauthorized);
        }
        return this.ApiOk(result, "OTP verified successfully");
    }
    #endregion

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var sessionId = User.Claims.FirstOrDefault(c => c.Type == "sessionId")?.Value;

        if (string.IsNullOrEmpty(sessionId))
        {
            return this.ApiError("Session ID not found in token", HttpStatusCode.Unauthorized);
        }
        await _authServices.Logout(sessionId);
        return this.ApiOk("Logout successfully");
    }

    [HttpGet("get-active-session")]
    [Authorize(Roles = "user")]
    public async Task<IActionResult> GetAllTheActiveSession()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.ApiError("User ID not found in token", HttpStatusCode.Unauthorized);
        }
        var sessions = await _authServices.GetAllSessionUser(userId);
        return this.ApiOk(sessions, "Active sessions retrieved successfully");
    }
}
