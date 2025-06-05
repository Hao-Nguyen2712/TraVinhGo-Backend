// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.OcopProduct.Mappers;
using TraVinhMaps.Application.Features.OcopProduct.Models;
using TraVinhMaps.Application.Features.OcopProduct;
using TraVinhMaps.Application.Features.OcopType.Interface;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Application.Features.OcopType.Models;
using TraVinhMaps.Application.Features.OcopType.Mappers;
using TraVinhMaps.Application.UnitOfWorks;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OcopTypeController : ControllerBase
{
    private readonly IRepository<Domain.Entities.OcopType> _repository;
    public OcopTypeController(IRepository<Domain.Entities.OcopType> repository)
    {
        _repository = repository;
    }
    [HttpGet]
    [Route("GetAllOcopType")]
    public async Task<IActionResult> GetAllOcopType()
    {
        var listOcopType = await _repository.ListAllAsync();
        return this.ApiOk(listOcopType);
    }
    [HttpGet]
    [Route("GetOcopTypeById/{id}", Name = "GetOcopTypeById")]
    public async Task<IActionResult> GetOcopTypeById(string id)
    {
        var ocopType = await _repository.GetByIdAsync(id);
        return this.ApiOk(ocopType);
    }
    [HttpGet]
    [Route("CountOcopTypes")]
    public async Task<IActionResult> CountOcopTypes()
    {
        var countOcopTypes = await _repository.CountAsync();
        return this.ApiOk(countOcopTypes);
    }
    [HttpPost]
    [Route("AddOcopType")]
    public async Task<IActionResult> AddOcopType([FromForm] CreateOcopTypeRequest createOcopTypeRequest)
    {
        var createOcopType = OcopTypeMapper.Mapper.Map<OcopType>(createOcopTypeRequest);
        createOcopType.OcopTypeStatus = true;
        var ocopType = await _repository.AddAsync(createOcopType);
        return CreatedAtRoute("GetOcopTypeById", new { id = ocopType.Id }, this.ApiOk(ocopType));
    }
    [HttpPut]
    [Route("UpdateOcopType")]
    public async Task<IActionResult> UpdateOcopType([FromBody] UpdateOcopTypeRequest updateOcopTypeRequest)
    {
        var existingOcopType = await _repository.GetByIdAsync(updateOcopTypeRequest.Id);
        if (existingOcopType == null)
        {
            throw new NotFoundException("Ocop type not found.");
        }

        existingOcopType.OcopTypeName = updateOcopTypeRequest.OcopTypeName;

        if (updateOcopTypeRequest.UpdateAt.HasValue)
        {
            existingOcopType.UpdateAt = updateOcopTypeRequest.UpdateAt.Value;
        }
        await _repository.UpdateAsync(existingOcopType);
        return this.ApiOk("Ocop type updated successfully.");
    }
}
