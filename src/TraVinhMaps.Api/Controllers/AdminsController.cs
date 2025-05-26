// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.Admins.Interface;
using TraVinhMaps.Application.Features.Admins.Models;
using TraVinhMaps.Application.Features.Roles.Interface;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AdminsController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IRoleService _roleService;

    public AdminsController(IAdminService adminService, IRoleService roleService)
    {
        _adminService = adminService;
        _roleService = roleService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllAdmin()
    {
        // Get the admin role from the role repository
        var adminRole = (await _roleService.ListAllAsync()).
            FirstOrDefault(r => r.RoleName.ToLower() == "admin" && r.RoleStatus);
        if (adminRole == null)
        {
            throw new NotFoundException("Admin role not found.");
        }

        // Get users whose RoleId matches the admin role
        var adminUsers = await _adminService.ListAsync(u => u.RoleId == adminRole.Id);
        return Ok(adminUsers);
    }

    [HttpGet("{id}", Name = "GetAdminById")]
    public async Task<IActionResult> GetAdminById(string id)
    {
        var admin = await _adminService.GetByIdAsync(id);
        return Ok(admin);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] AdminRequest request)
    {
        var adminRole = (await _roleService.ListAllAsync())
       .FirstOrDefault(r => r.RoleName.ToLower() == "admin" && r.RoleStatus);

        if (adminRole == null)
        {
            throw new NotFoundException("Admin role not found.");
        }
        request.RoleId = adminRole.Id;
        var createdAdmin = await _adminService.AddAsync(request);
        return CreatedAtRoute(nameof(GetAdminById), new { id = createdAdmin.Id }, createdAdmin);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateAdminRequest entity)
    {
        //var existingAdmin = await _adminService.GetByIdAsync(id);
        //if(existingAdmin == null)
        //{
        //    throw new NotFoundException("");
        //}
        var updatedAdmin = await _adminService.UpdateAsync(entity);
        return Ok(updatedAdmin);
    }

    [HttpDelete("LockAdmin/{id}")]
    public async Task<IActionResult> DeleteAdmin(string id)
    {
        var admin = await _adminService.DeleteAdmin(id);
        if (admin != false)
        {
            return Ok(admin);
        }
        throw new BadRequestException("Failed to lock admin");
    }

    [HttpPut("RestoreAdmin/{id}")]
    public async Task<IActionResult> RestoreAdmin(string id)
    {
        var admin = await _adminService.RestoreAdmin(id);
        return Ok(admin);
    }
}
