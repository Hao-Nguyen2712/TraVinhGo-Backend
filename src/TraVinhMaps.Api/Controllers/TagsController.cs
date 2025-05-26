// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.Tags.Interface;
using TraVinhMaps.Application.UnitOfWorks;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllTags()
    {
        var tags = await _tagService.ListAllAsync();
        return Ok(tags);
    }

    [HttpGet("GetTagById/{id}")]
    public async Task<IActionResult> GetTagById(string id)
    {
        var tag = await _tagService.GetByIdAsync(id);

        if (tag == null)
        {
            throw new NotFoundException("Tag not found!");
        }
        return Ok(tag);
    }
}
