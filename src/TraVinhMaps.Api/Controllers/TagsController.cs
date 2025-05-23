// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.UnitOfWorks;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TagsController : ControllerBase
{
    private readonly IRepository<TraVinhMaps.Domain.Entities.Tags> _tagRepository;

    public TagsController(IRepository<TraVinhMaps.Domain.Entities.Tags> tagRepository)
    {
        _tagRepository = tagRepository;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllTags()
    {
        var tags = await _tagRepository.ListAllAsync();
        return Ok(tags);
    }

    [HttpGet("GetTagById/{id}")]
    public async Task<IActionResult> GetTagById(string id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);

        if (tag == null)
        {
            throw new NotFoundException("Tag not found!");
        }
        return Ok(tag);
    }
}
