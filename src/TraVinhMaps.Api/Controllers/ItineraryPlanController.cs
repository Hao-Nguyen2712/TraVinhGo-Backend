// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.ItineraryPlan.Interface;
using TraVinhMaps.Application.Features.ItineraryPlan.Mappers;
using TraVinhMaps.Application.Features.ItineraryPlan.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ItineraryPlanController : ControllerBase
{
    private readonly IItineraryPlanService _itineraryPlanService;

    public ItineraryPlanController(IItineraryPlanService itineraryPlanService)
    {
        _itineraryPlanService = itineraryPlanService;
    }

    [HttpGet]
    [Route("GetAllItineraryPlan")]
    public async Task<IActionResult> GetAllItineraryPlan()
    {
        var listItineraryPlan = await this._itineraryPlanService.ListAllAsync();
        return this.ApiOk(listItineraryPlan);
    }

    [HttpGet]
    [Route("[action]/{id}", Name = "GetItineraryPlanById")]
    public async Task<IActionResult> GetItineraryPlanById(string id)
    {
        var itineraryPlanDetail = await this._itineraryPlanService.GetByIdAsync(id);
        return this.ApiOk(itineraryPlanDetail);
    }

    [HttpPost]
    [Route("CreateItineraryPlan")]
    public async Task<IActionResult> CreateItineraryPlan(ItineraryPlanRequest itineraryPlanRequest)
    {
        if(itineraryPlanRequest.Locations.Count< 2)
        {
            return this.ApiError("The itinerary must have at least 2 addresses.");
        }
        var itineraryPlan = ItineraryPlanMapper.Mapper.Map<ItineraryPlan>(itineraryPlanRequest);
        var itineraryPlanCreated = await _itineraryPlanService.AddAsync(itineraryPlan);
        return CreatedAtRoute("GetItineraryPlanById", new { id = itineraryPlanCreated.Id }, this.ApiOk(itineraryPlanCreated));
    }

    [HttpPut]
    [Route("UpdateItineraryPlan")]
    public async Task<IActionResult> UpdateItineraryPlan(UpdateItineraryPlanRequest updateItineraryPlanRequest)
    {
        if(updateItineraryPlanRequest == null)
        {
            return this.ApiError("Object can't be null");
        }
        var itineraryPlan = await _itineraryPlanService.GetByIdAsync(updateItineraryPlanRequest.Id);
        if(itineraryPlan == null)
        {
            throw new NotFoundException("No itineraryPlan was found");
        }
        itineraryPlan.Name = updateItineraryPlanRequest.Name;
        itineraryPlan.Duration = updateItineraryPlanRequest.Duration;
        itineraryPlan.EstimatedCost = updateItineraryPlanRequest.EstimatedCost;
        itineraryPlan.UpdateAt = DateTime.Now.ToLocalTime();
        await this._itineraryPlanService.UpdateAsync(itineraryPlan);
        return CreatedAtRoute("GetItineraryPlanById", new { id = itineraryPlan.Id }, this.ApiOk(itineraryPlan));
    }

    [HttpDelete]
    [Route("DeleteItineraryPlan")]
    public async Task<IActionResult> DeleteItineraryPlan(string id)
    {
        if (id == null)
        {
            return this.ApiError("id can't be null");
        }
        var itineraryPlan = await _itineraryPlanService.GetByIdAsync(id);
        if (itineraryPlan == null)
        {
            throw new NotFoundException("No Destination was found");
        }
        if (itineraryPlan.Status == false)
        {
            return this.ApiError("Itinerary plan is already inactive");
        }
        itineraryPlan.Status = false;
        itineraryPlan.UpdateAt = DateTime.Now.ToLocalTime();
        await this._itineraryPlanService.UpdateAsync(itineraryPlan);
        //return NoContent();
        return this.ApiOk("Itinerary plan deleted successfully");
    }

    [HttpPut]
    [Route("RestoreItineraryPlan/{id}")]
    public async Task<IActionResult> RestoreItineraryPlan(string id)
    {
        if (id == null)
        {
            return this.ApiError("id can't be null");
        }
        var itineraryPlan = await _itineraryPlanService.GetByIdAsync(id);
        if (itineraryPlan == null)
        {
            throw new NotFoundException("No Destination was found");
        }
        if (itineraryPlan.Status == true)
        {
            return this.ApiError("Itinerary plan is already inactive");
        }
        itineraryPlan.Status = true;
        itineraryPlan.UpdateAt = DateTime.Now.ToLocalTime();
        await this._itineraryPlanService.UpdateAsync(itineraryPlan);
        //return NoContent();
        return this.ApiOk("Itinerary plan restor successfully");
    }
}
