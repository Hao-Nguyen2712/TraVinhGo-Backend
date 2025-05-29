// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.Tags.Interface;

namespace TraVinhMaps.Api.Controllers;

// Controller for managing tag-related operations
[Route("api/[controller]")]
[ApiController]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    // Constructor - injects the ITagService
    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    // GET: api/Tags/all
    // Retrieves a list of all tags from the system
    [HttpGet("all")]
    public async Task<IActionResult> GetAllTags()
    {
        var tags = await _tagService.ListAllAsync();
        return Ok(tags);
    }

    // GET: api/Tags/GetTagById/{id}
    // Retrieves a specific tag by its ID
    [HttpGet("GetTagById/{id}")]
    public async Task<IActionResult> GetTagById(string id)
    {
        var tag = await _tagService.GetByIdAsync(id);

        // If the tag does not exist, throw a 404 Not Found
        if (tag == null)
        {
            throw new NotFoundException("Tag not found!");
        }

        return Ok(tag);
    }
}
