// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.Roles.Interface;
using TraVinhMaps.Application.Features.Roles.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _roleService.ListAllAsync();
        return this.ApiOk(list);
    }

    [HttpGet("{id}", Name = "GetById")]
    public async Task<IActionResult> GetById(string id)
    {
        var role = await _roleService.GetByIdAsync(id);
        if (role == null)
        {
            throw new NotFoundException($"Role with ID '{id}' not found.");
        }
        return Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoleRequest role)
    {
        var createdRole = await _roleService.AddAsync(role);
        return CreatedAtRoute(nameof(GetById), new { id = createdRole.Id }, createdRole);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] RoleRequest roleRequest)
    {
        var result = await _roleService.UpdateAsync(id, roleRequest);
        if (!result)
        {
            throw new NotFoundException($"No role found with ID: {id}");
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _roleService.DeleteAsync(id);
        return NoContent();
    }

}
