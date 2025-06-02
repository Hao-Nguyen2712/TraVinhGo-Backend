// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.DestinationTypes.Interface;
using TraVinhMaps.Application.Features.DestinationTypes.Mappers;
using TraVinhMaps.Application.Features.DestinationTypes.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class DestinationTypeController : ControllerBase
{
    private readonly IDestinationTypeService _destinationTypeService;

    public DestinationTypeController(IDestinationTypeService destinationTypeService)
    {
        _destinationTypeService = destinationTypeService;
    }

    [HttpGet]
    [Route("GetAllDestinationTypes")]
    public async Task<IActionResult> GetAllDestinationTypes()
    {
        var destinationTypes = await _destinationTypeService.ListAllAsync();
        return this.ApiOk(destinationTypes);
    }

    [HttpGet]
    [Route("GetAllActiveDestinationTypes")]
    public async Task<IActionResult> GetAllActiveDestinationTypes()
    {
        var activeDestinationTypes = await _destinationTypeService.ListAsync(d => d.Status == true);
        return this.ApiOk(activeDestinationTypes);
    }

    [HttpGet]
    [Route("[action]/{id}", Name = "GetDestinationTypeById")]
    public async Task<IActionResult> GetDestinationTypeById(string id)
    {
        var destinationType = await _destinationTypeService.GetByIdAsync(id);
        if (destinationType == null)
        {
            throw new NotFoundException("Destination type not found");
        }
        return this.ApiOk(destinationType);
    }

    [HttpPost]
    [Route("CreateDestinationType")]
    public async Task<IActionResult> CreateDestinationType([FromBody] DestinationTypeRequest destinationTypeRequest)
    {
        if (destinationTypeRequest == null)
        {
            throw new BadRequestException("Invalid destination type data");
        }
        var newDestinationType = DestinationTypeMapper.Mapper.Map<DestinationType>(destinationTypeRequest);
        var destinationType = await _destinationTypeService.AddAsync(newDestinationType);
        return CreatedAtRoute("GetDestinationTypeById", new { id = destinationType.Id }, this.ApiOk(destinationType));
    }

    [HttpPut]
    [Route("UpdateDestinationType")]
    public async Task<IActionResult> UpdateDestinationType([FromBody] UpdateDestinationTypeRequest updateDestinationTypeRequest)
    {
        if (updateDestinationTypeRequest == null)
        {
            throw new BadRequestException("Invalid destination type data");
        }
        var existingDestinationType = await _destinationTypeService.GetByIdAsync(updateDestinationTypeRequest.Id);
        if (existingDestinationType == null)
        {
            throw new NotFoundException("Destination type not found");
        }
        existingDestinationType.Name = updateDestinationTypeRequest.Name;
        existingDestinationType.MarkerId = updateDestinationTypeRequest.MarkerId;
        existingDestinationType.UpdateAt = DateTime.UtcNow.ToLocalTime();
        await _destinationTypeService.UpdateAsync(existingDestinationType);
        return CreatedAtRoute("GetDestinationTypeById", new { id = existingDestinationType.Id }, this.ApiOk(existingDestinationType));
    }

    [HttpDelete]
    [Route("[action]/{id}", Name = "DeleteDestinationType")]
    public async Task<IActionResult> DeleteDestinationType(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return this.ApiError("id can't be null or empty");
        }

        var destinationType = await _destinationTypeService.GetByIdAsync(id);
        if (destinationType == null)
        {
            throw new NotFoundException("Destination type not found");
        }

        if (destinationType.Status == false)
        {
            return this.ApiError("Destination type is already inactive");
        }

        destinationType.Status = false;
        await _destinationTypeService.UpdateAsync(destinationType);
        return CreatedAtRoute("GetDestinationTypeById", new { id = destinationType.Id }, this.ApiOk(destinationType));
    }

    [HttpPut]
    [Route("RestoreDestinationType/{id}")]
    public async Task<IActionResult> RestoreDestinationType(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return this.ApiError("id can't be null or empty");
        }

        var destinationType = await _destinationTypeService.GetByIdAsync(id);
        if (destinationType == null)
        {
            throw new NotFoundException("Destination type not found");
        }

        if (destinationType.Status == true)
        {
            return this.ApiError("Destination type is already active");
        }

        destinationType.Status = true;
        await _destinationTypeService.UpdateAsync(destinationType);
        return this.ApiOk("Destination type restored successfully");
    }
}
