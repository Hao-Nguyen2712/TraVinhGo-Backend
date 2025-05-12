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

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var list = await _userService.ListAllAsync();
        return Ok(list);
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
        if(existingUser == null){
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
