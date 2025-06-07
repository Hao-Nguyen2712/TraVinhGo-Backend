// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.SellingLink.Interface;
using TraVinhMaps.Application.Features.SellingLink.Mappers;
using TraVinhMaps.Application.Features.SellingLink.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SellingLinkController : ControllerBase
{
    private readonly ISellingLinkService _service;
    public SellingLinkController(ISellingLinkService service)
    {
        _service = service;
    }
    [HttpGet]
    [Route("GetAllSellingLink")]
    public async Task<IActionResult> GetAllSellingLink()
    {
        var listSellingLink = await _service.ListAllAsync();
        return this.ApiOk(listSellingLink);
    }
    [HttpGet]
    [Route("GetSellingLinkById/{id}", Name = "GetSellingLinkById")]
    public async Task<IActionResult> GetSellingLinkById(string id)
    {
        var sellingLink = await _service.GetByIdAsync(id);
        return this.ApiOk(sellingLink);
    }
    [HttpGet]
    [Route("CountSellingLinks")]
    public async Task<IActionResult> CountSellingLinks()
    {
        var countSellingLinks = await _service.CountAsync();
        return this.ApiOk(countSellingLinks);
    }
    [HttpPost]
    [Route("AddSellingLink")]
    public async Task<IActionResult> AddSellingLink([FromForm] CreateSellingLinkRequest createSellingLinkRequest)
    {
        var createSellingLink = SellingLinkMapper.Mapper.Map<SellingLink>(createSellingLinkRequest);
        var sellingLink = await _service.AddAsync(createSellingLink);
        return CreatedAtRoute("GetSellingLinkById", new { id = sellingLink.Id }, this.ApiOk(sellingLink));
    }
    [HttpPut]
    [Route("UpdateSellingLink")]
    public async Task<IActionResult> UpdateSellingLink([FromBody] UpdateSellingLinkRequest updateSellingLinkRequest)
    {
        var existingSellingLink = await _service.GetByIdAsync(updateSellingLinkRequest.Id);
        if (existingSellingLink == null)
        {
            throw new NotFoundException("Selling link not found.");
        }

        existingSellingLink.Tittle = updateSellingLinkRequest.Tittle;
        existingSellingLink.Link = updateSellingLinkRequest.Link;

        if (updateSellingLinkRequest.UpdateAt.HasValue)
        {
            existingSellingLink.UpdateAt = updateSellingLinkRequest.UpdateAt.Value;
        }
        await _service.UpdateAsync(existingSellingLink);
        return this.ApiOk("Selling link updated successfully.");
    }
    [HttpDelete]
    [Route("DeleteSellingLink/{id}")]
    public async Task<IActionResult> DeleteSellingLink(SellingLink sellingLink)
    {
        var ocopProduct = await _service.GetByIdAsync(sellingLink.Id);
        if (ocopProduct == null)
        {
            throw new NotFoundException("Selling link not found.");
        }
        await _service.DeleteAsync(ocopProduct);
        return this.ApiOk("Selling link deleted successfully.");
    }
}
