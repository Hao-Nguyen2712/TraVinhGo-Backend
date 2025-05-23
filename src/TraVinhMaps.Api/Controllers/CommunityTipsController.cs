// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Application.Features.CommunityTips.Interface;
using TraVinhMaps.Application.Features.CommunityTips.Models;
using TraVinhMaps.Application.Features.CommunityTips.Mappers;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CommunityTipsController : ControllerBase
{
    private readonly ICommunityTipsService _service;
    public CommunityTipsController(ICommunityTipsService service)
    {
        _service = service;
    }
    [HttpGet]
    [Route("GetAllTipActive")]
    public async Task<IActionResult> GetAllTipActive()
    {
<<<<<<< Updated upstream
        var getAllTipActive = await _service.ListAsync(tip => tip.Status == true);
=======
        var getAllTipActive = await _service.ListAsync(t => t.Status == true);
>>>>>>> Stashed changes
        return this.ApiOk(getAllTipActive);
    }

    [HttpGet]
    [Route("GetAllTip")]
    public async Task<IActionResult> GetAllTip()
    {
        var listTip = await _service.ListAllAsync();
        return Ok(listTip);
    }
    [HttpGet]
    [Route("GetByIdTip/{id}", Name = "GetByIdTip")]
    public async Task<IActionResult> GetByIdTip(string id)
    {
        var tip = await _service.GetByIdAsync(id);
        return Ok(tip);
    }
  
    [HttpGet]
    [Route("CountTips")]
    public async Task<IActionResult> CountTips()
    {
        var countTips = await _service.CountAsync();
        return this.ApiOk(countTips);
    }
    [HttpPost]
    [Route("AddTip")]
    public async Task<IActionResult> AddTip([FromForm] CreateCommunityTipRequest createCommunityTipRequest)
    {
<<<<<<< Updated upstream
        var addTip = CommunityTipsMapper.Mapper.Map<Tips>(createCommunityTipRequest);
        var tip = await _service.AddAsync(addTip);
=======
        // check for existing tip with same title and tag
        var existingTips = await _service.ListAsync(t => t.Title.ToLower().Trim() == createCommunityTipRequest.Title.ToLower().Trim() &&
        t.TagId == createCommunityTipRequest.TagId);

        if(existingTips.Any())
        {
            throw new BadRequestException("A tip with the same title and tag already exists.");
        }

        var createTip = CommunityTipsMapper.Mapper.Map<Tips>(createCommunityTipRequest);
        var tip = await _service.AddAsync(createTip);
>>>>>>> Stashed changes
        tip.Status = true;
        return CreatedAtRoute("GetByIdTip", new { id = tip.Id }, tip);
    }
    [HttpPut]
    [Route("UpdateTip")]
    public async Task<IActionResult> UpdateTip([FromBody] UpdateCommunityTipRequest updateCommunityTipRequest)
    {
        var updateTip = CommunityTipsMapper.Mapper.Map<Tips>(updateCommunityTipRequest);
        await _service.UpdateAsync(updateTip);
        return this.ApiOk("Updated tip successfully.");
    }
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
        return this.ApiOk("Restore tip successfully.");
    }
}
