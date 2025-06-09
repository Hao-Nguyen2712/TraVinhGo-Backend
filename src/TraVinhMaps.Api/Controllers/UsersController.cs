// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.Roles.Interface;
using TraVinhMaps.Application.Features.Users;
using TraVinhMaps.Application.Features.Users.Interface;
using TraVinhMaps.Application.Features.Users.Mappers;
using TraVinhMaps.Application.Features.Users.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly UploadImageUser _uploadImageUser;
    private readonly IRoleService _roleService;

    public UsersController(IUserService userService, UploadImageUser uploadImageUser, IRoleService roleService)
    {
        _userService = userService;
        _uploadImageUser = uploadImageUser;
        _roleService = roleService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers()
    {
        var list = await _userService.ListAllAsync();
        return Ok(list);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveUsers()
    {
        var user = await _userService.ListAsync(u => u.Status == true);
        return Ok(user);
    }

    [HttpGet("inActive")]
    public async Task<IActionResult> GetInActiveUsers()
    {
        var inActiveUser = await _userService.ListAsync(u => u.Status == false);
        return Ok(inActiveUser);
    }

    [HttpGet("{id}", Name = "GetUserById")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException("User not found!");
        }
        return Ok(user);
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCountUsers()
    {
        var countUser = await _userService.CountAsync();
        return Ok(countUser);
    }

    [HttpGet("count-active")]
    public async Task<IActionResult> GetCountActiveUsers()
    {
        var countUser = await _userService.CountAsync(u => u.Status == true && u.IsForbidden == false);
        return Ok(countUser);
    }

    [HttpGet("count-inActive")]
    public async Task<IActionResult> GetCountInActiveUsers()
    {
        var countUser = await _userService.CountAsync(u => u.Status == false && u.IsForbidden == true);
        return Ok(countUser);
    }

    [HttpGet("count/banned")]
    public async Task<IActionResult> GetBannedUsersCount()
    {
        var banUserCount = await _userService.CountAsync(u => u.IsForbidden == true);
        return Ok(banUserCount);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        var createUser = await _userService.AddAsync(user);
        return CreatedAtRoute(nameof(GetUserById), new { id = user.Id }, user);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromForm] UserRequest userRequest, IFormFile? imageFile)
    {
        var existingUser = await _userService.GetByIdAsync(userRequest.Id);
        if (existingUser == null)
        {
            throw new NotFoundException("User not found!");
        }

        var updatedUser = UserMapper.Mapper.Map(userRequest, existingUser);

        if (imageFile != null)
        {
            var avatarUrl = await _uploadImageUser.UploadImage(imageFile);
            updatedUser.Profile = updatedUser.Profile ?? new Profile();
            updatedUser.Profile.Avatar = avatarUrl;
        }

        await _userService.UpdateAsync(updatedUser);
        return Ok(updatedUser);
    }

    [HttpDelete("LockUser/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userService.DeleteUser(id);
        if (user != false)
        {
            return Ok(user);
        }
        throw new BadRequestException("Failed to lock account");
    }

    [HttpPut("RestoreUser/{id}")]
    public async Task<IActionResult> RestoreUser(string id)
    {
        var user = await _userService.RestoreUser(id);
        return Ok(user);
    }

    [Authorize]
    [HttpGet("get-profile-admin")]
    public async Task<IActionResult> GetProfileAdmin()
    {
        var sessionId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        // var sessionId = "6832ba4fbd35e0ad520794c8";
        if (string.IsNullOrEmpty(sessionId))
        {
            return this.ApiError("Session ID not found in token", HttpStatusCode.Unauthorized);
        }
        var result = await _userService.GetProfileAdmin(sessionId);
        if (result == null)
        {
            return this.ApiError("User not found", HttpStatusCode.NotFound);
        }
        return this.ApiOk(result, "Get profile admin successfully");
    }
    [Authorize(Roles = "admin, super-admin")]
    [HttpPost("update-profile-admin")]
    public async Task<IActionResult> UpdateProfileAdmin([FromForm] UpdateProfileAdminRequest request)
    {
        if (request == null)
        {
            return this.ApiError("Update fail", HttpStatusCode.BadRequest);
        }
        await _userService.UpdateProfileAdmin(request);
        return this.ApiOk("", "Update profile admin successfully");
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetUserStatistics([FromQuery] string groupBy = "all", [FromQuery] string timeRange = "month")
    {
        var stats = await _userService.GetUserStatisticsAsync(groupBy, timeRange);
        return this.ApiOk(stats, "User statistics retrieved successfully");
    }

}
