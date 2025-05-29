// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.Admins.Interface;
using TraVinhMaps.Application.Features.Admins.Models;

namespace TraVinhMaps.Api.Controllers;

// Controller for managing admin users via API
[Route("api/[controller]")]
[ApiController]
public class AdminsController : ControllerBase
{
    private readonly IAdminService _adminService;

    // Constructor - injects the admin service
    public AdminsController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // GET: api/Admins/all
    // Retrieves a list of all admin users
    [HttpGet("all")]
    public async Task<IActionResult> GetAllAdmin()
    {
        var adminUsers = await _adminService.ListAllAsync();
        return Ok(adminUsers);
    }

    // GET: api/Admins/{id}
    // Retrieves details of a specific admin user by their ID
    [HttpGet("{id}", Name = "GetAdminById")]
    public async Task<IActionResult> GetAdminById(string id)
    {
        var admin = await _adminService.GetByIdAsync(id);
        return Ok(admin);
    }

    // POST: api/Admins/create
    // Creates a new admin user
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] AdminRequest request)
    {
        try
        {
            var createdAdmin = await _adminService.AddAsync(request);
            // Returns 201 Created with route to the new admin resource
            return CreatedAtRoute(nameof(GetAdminById), new { id = createdAdmin.Id }, createdAdmin);
        }
        catch (InvalidOperationException ex)
        {
            // Handles business logic error (e.g., email already exists)
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            // Handles validation error (e.g., invalid or missing fields)
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // Handles any other unexpected error
            return StatusCode(500, new { message = "Failed to create admin" });
        }
    }

    // PUT: api/Admins
    // Updates an existing admin's information
    //[HttpPut]
    //public async Task<IActionResult> Update([FromBody] UpdateAdminRequest entity)
    //{
    //    var updatedAdmin = await _adminService.UpdateAsync(entity);
    //    return Ok(updatedAdmin);
    //}


    // DELETE: api/Admins/LockAdmin/{id}
    // Locks (or soft deletes) an admin user by ID
    [HttpDelete("LockAdmin/{id}")]
    public async Task<IActionResult> DeleteAdmin(string id)
    {
        var admin = await _adminService.DeleteAdmin(id);
        if (admin != false)
        {
            return Ok(admin); // Successfully locked the admin
        }
        // Locking failed, return bad request
        throw new BadRequestException("Failed to lock admin");
    }

    // PUT: api/Admins/RestoreAdmin/{id}
    // Restores a previously locked/deactivated admin user
    [HttpPut("RestoreAdmin/{id}")]
    public async Task<IActionResult> RestoreAdmin(string id)
    {
        var admin = await _adminService.RestoreAdmin(id);
        return Ok(admin);
    }
}
