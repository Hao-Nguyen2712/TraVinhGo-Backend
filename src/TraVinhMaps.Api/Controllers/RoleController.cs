// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.Roles.Interface;

namespace TraVinhMaps.Api.Controllers;

// Controller responsible for handling Role-related API requests
[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    // Constructor - injects the IRoleService dependency
    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    // GET: api/Role
    // Retrieves all roles
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _roleService.ListAllAsync();
        return this.ApiOk(list);
    }

    // GET: api/Role/{id}
    // Retrieves a specific role by its ID
    [HttpGet("{id}", Name = "GetById")]
    public async Task<IActionResult> GetById(string id)
    {
        var role = await _roleService.GetByIdAsync(id);

        // If the role is not found, throw a 404 Not Found exception
        if (role == null)
        {
            throw new NotFoundException($"Role with ID '{id}' not found.");
        }

        return Ok(role);
    }
}
