// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Application.Features.Destination.Interface;
using TraVinhMaps.Application.Features.Destination;
using TraVinhMaps.Application.Features.Destination.Models;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Application.Features.Destination.Mappers;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TouristDestinationController : ControllerBase
{
    private readonly ITouristDestinationService _touristDestinationService;
    private readonly ImageManagementDestinationServices _imageManagementDestinationServices;

    public TouristDestinationController(ITouristDestinationService touristDestinationService, ImageManagementDestinationServices imageManagementDestinationServices)
    {
        _touristDestinationService = touristDestinationService;
        _imageManagementDestinationServices = imageManagementDestinationServices;
    }

    [HttpGet]
    [Route("GetAllProducts")]
    public async Task<IActionResult> GetAllDestinations()
    {
        var list = await this._touristDestinationService.ListAllAsync();
        return Ok(list);
    }

    [HttpPost]
    [Route("CreateDestination")]
    public async Task<IActionResult> CreateDestination([FromBody] TouristDestinationRequest touristDestination)
    {
        var touristDestination1 = DestinationMapper.Mapper.Map<TouristDestination>(touristDestination);
        var destination = await this._touristDestinationService.AddAsync(touristDestination1);
        return Ok(destination);
    }

    //[HttpPost]
    //[Route("AddDestinationImage")]
    //public async Task<IActionResult> AddDestinationImage([FromForm] AddImageRequest addImageRequest)
    //{
    //    var linkImage = await _imageManagementDestinationServices.AddImageDestination(addImageRequest.imageFile);
    //    return Ok(linkImage);
    //}
}
