using CloudinaryDotNet;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Application.Features.Users;
using TraVinhMaps.Application.Features.Users.Interface;
using TraVinhMaps.Application.Features.Users.Mappers;
using TraVinhMaps.Application.Features.Users.Models;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Domain.Specs;

namespace TraVinhMaps.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly UploadImageUser _uploadImageUser;

    // private readonly IValidator<User> _userValidator;

    public UsersController(IUserService userService, UploadImageUser uploadImageUser)
    {
        _userService = userService;
        _uploadImageUser = uploadImageUser;
        // _userValidator = userValidator;
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

    [HttpGet("paging")]
    public async Task<IActionResult> GetUserPaging([FromQuery] UserSpecParams userSpecParams)
    {
        var list = await _userService.GetUsersAsync(userSpecParams);
        return Ok(list);
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
            return NotFound();
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
            return NotFound();
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

    [HttpDelete("LockUer/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userService.DeleteUser(id);
        if (user != false)
        {
            return Ok(user);
        }
        return BadRequest("Failed to lock account");
    }

    [HttpPut("RestoreUser/{id}")]
    public async Task<IActionResult> RestoreUser(string id)
    {
        var user = await _userService.RestoreUser(id);
        return Ok(user);
    }

}
