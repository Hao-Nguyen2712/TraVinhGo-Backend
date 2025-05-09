// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("user-register")]
    public async Task<IActionResult> Register(string phoneNumber)
    {
        await _authServices.RequestMobileRegistrationOtp(phoneNumber);
        return Ok();
    }
}
