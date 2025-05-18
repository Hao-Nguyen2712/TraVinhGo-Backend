// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        if (email == null)
        {
            return this.ApiError("Email is required", HttpStatusCode.BadRequest);
        }
        var result = await _authServices.AuthenWithEmail(email);
        var response = new
        {
            token = result
        };
        return this.ApiOk(JsonConvert.SerializeObject(response), "Email authentication requested successfully");
    }

    [HttpPost("request-phonenumber-authen")]
    public async Task<IActionResult> AuthenWithPhoneNumber(string phoneNumber)
    {
        if (phoneNumber == null)
        {
            return this.ApiError("Phone number is required", HttpStatusCode.BadRequest);
        }
        var result = await _authServices.AuthenWithPhoneNumber(phoneNumber);
        var response = new
        {
            token = result
        };
        return this.ApiOk(JsonConvert.SerializeObject(response)
        , "Phone number authentication requested successfully");
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
        return this.ApiOk(JsonConvert.SerializeObject(result), "OTP verified successfully");
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

    [HttpPost("refresh-otp")]
    public async Task<IActionResult> RefreshOtp(string item)
    {
        if (string.IsNullOrEmpty(item))
        {
            return this.ApiError("Item need require", HttpStatusCode.BadRequest);
        }
        var result = await _authServices.RefreshOtp(item);
        var response = new
        {
            token = result
        };
        return this.ApiOk(JsonConvert.SerializeObject(response), "OTP refreshed successfully");
    }
}
