// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.CommunityTips.Interface;
using TraVinhMaps.Application.Features.CommunityTips.Mappers;
using TraVinhMaps.Application.Features.CommunityTips.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Api.Controllers;

// Controller for managing community tips via API
[Route("api/[controller]")]
[ApiController]
public class CommunityTipsController : ControllerBase
{
    private readonly ICommunityTipsService _service;

    // Constructor - injects the community tips service
    public CommunityTipsController(ICommunityTipsService service)
    {
        _service = service;
    }

    // GET: api/CommunityTips/GetAllTipActive
    // Retrieves all community tips that are marked as active (Status == true)
    [HttpGet]
    [Route("GetAllTipActive")]
    public async Task<IActionResult> GetAllTipActive()
    {
        var getAllTipActive = await _service.ListAsync(t => t.Status == true);
        return this.ApiOk(getAllTipActive); // Custom extension method to return standardized API responses
    }

    // GET: api/CommunityTips/GetAllTip
    // Retrieves all community tips regardless of their status
    [HttpGet]
    [Route("GetAllTip")]
    public async Task<IActionResult> GetAllTip()
    {
        var listTip = await _service.ListAllAsync();
        return Ok(listTip);
    }

    // GET: api/CommunityTips/GetByIdTip/{id}
    // Retrieves a specific tip by its ID
    [HttpGet]
    [Route("GetByIdTip/{id}", Name = "GetByIdTip")]
    public async Task<IActionResult> GetByIdTip(string id)
    {
        var tip = await _service.GetByIdAsync(id);
        return Ok(tip);
    }

    // GET: api/CommunityTips/CountTips
    // Returns the total number of community tips
    [HttpGet]
    [Route("CountTips")]
    public async Task<IActionResult> CountTips()
    {
        var countTips = await _service.CountAsync();
        return this.ApiOk(countTips);
    }

    // POST: api/CommunityTips/CreateTip
    // Creates a new community tip
    [HttpPost]
    [Route("CreateTip")]
    public async Task<IActionResult> CreateTip([FromBody] CreateCommunityTipRequest createCommunityTipRequest)
    {
        var tip = await _service.AddAsync(createCommunityTipRequest);
        return CreatedAtRoute("GetByIdTip", new { id = tip.Id }, tip); // Returns 201 Created with the route to the new tip
    }

    // PUT: api/CommunityTips/UpdateTip
    // Updates an existing community tip
    [HttpPut]
    [Route("UpdateTip")]
    public async Task<IActionResult> UpdateTip([FromBody] UpdateCommunityTipRequest updateCommunityTipRequest)
    {
        // Check if the tip exists
        var oldTip = await _service.GetByIdAsync(updateCommunityTipRequest.Id);
        if (oldTip == null)
            throw new NotFoundException("Tip not found.");

        // Map updated data and retain the original status
        var updateTip = CommunityTipsMapper.Mapper.Map<Tips>(updateCommunityTipRequest);
        updateTip.Status = oldTip.Status;

        await _service.UpdateAsync(updateTip);
        return this.ApiOk("Updated tip successfully.");
    }

    // DELETE: api/CommunityTips/DeleteTip/{id}
    // Deletes a specific community tip by its ID
    [HttpDelete]
    [Route("DeleteTip/{id}")]
    public async Task<IActionResult> DeleteTip(string id)
    {
        var tip = await _service.GetByIdAsync(id);
        if (tip == null)
        {
            throw new NotFoundException("Tip not found.");
        }
        await _service.DeleteTipAsync(id);
        return this.ApiOk("Deleted tip successfully.");
    }

    // PUT: api/CommunityTips/RestoreTip/{id}
    // Restores a previously deleted or deactivated tip
    [HttpPut]
    [Route("RestoreTip/{id}")]
    public async Task<IActionResult> RestoreTip(string id)
    {
        var restoreTip = await _service.GetByIdAsync(id);
        if (restoreTip == null)
        {
            throw new NotFoundException("Tip not found.");
        }
        await _service.RestoreTipAsync(id);
        return this.ApiOk("Restored tip successfully.");
    }
}
