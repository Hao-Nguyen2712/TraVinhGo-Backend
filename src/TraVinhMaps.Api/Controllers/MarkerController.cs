// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.Markers;
using TraVinhMaps.Application.Features.Markers.Interface;
using TraVinhMaps.Application.Features.Markers.Mappers;
using TraVinhMaps.Application.Features.Markers.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class MarkerController : ControllerBase
{
    private readonly IMarkerService _markerService;
    private readonly ImageManagementMarkerServices _imageManagementMarkerServices;
    public MarkerController(IMarkerService markerService, ImageManagementMarkerServices imageManagementMarkerServices)
    {
        _markerService = markerService;
        _imageManagementMarkerServices = imageManagementMarkerServices;
    }

    [HttpGet]
    [Route("GetAllMarkers")]
    public async Task<IActionResult> GetAllMarkers()
    {
        var markers = await _markerService.ListAllAsync();
        return this.ApiOk(markers);
    }

    [HttpGet]
    [Route("[action]/{id}", Name = "GetMarkerById")]
    public async Task<IActionResult> GetMarkerById(string id)
    {
        var marker = await _markerService.GetByIdAsync(id);
        return this.ApiOk(marker);
    }

    [HttpPost]
    [Route("CreateMarker")]
    public async Task<IActionResult> CreateMarker([FromForm] CreateMarkerRequest createMarkerRequest)
    {
        if (createMarkerRequest == null)
        {
            return this.ApiError("Object can't be null");
        }

        if(createMarkerRequest.ImageFile == null || createMarkerRequest.ImageFile.Length == 0)
        {
            return this.ApiError("ImageFile is required");
        }
        var urlImage = await _imageManagementMarkerServices.AddImageMarker(createMarkerRequest.ImageFile);   
        if(urlImage == null)
        {
            return this.ApiError("Error uploading image");
        }
        var newMarker = MarkerMapper.Mapper.Map<Marker>(createMarkerRequest);
        newMarker.Image = urlImage;
        var marker = await _markerService.AddAsync(newMarker);
        if (marker == null)
        {
            return this.ApiError("Error creating marker");
        }
        return CreatedAtRoute("GetMarkerById", new { id = marker.Id }, this.ApiOk(marker));
    }

    [HttpPut]
    [Route("UpdateMarker")]
    public async Task<IActionResult> UpdateMarker([FromForm] UpdateMarkerRequest updateMarkerRequest)
    {
        if (updateMarkerRequest == null)
        {
            return this.ApiError("Object can't be null");
        }

        var marker = await _markerService.GetByIdAsync(updateMarkerRequest.Id);
        if (marker == null)
        {
            throw new NotFoundException("Marker not found");
        }

        marker.Name = updateMarkerRequest.Name;

        await _markerService.UpdateAsync(marker);
        return CreatedAtRoute("GetMarkerById", new { id = marker.Id }, this.ApiOk(marker));
    }

    [HttpPut]
    [Route("EditMarkerImage")]
    public async Task<IActionResult> EditMarkerImage(EditMarkerPictureRequest editMarkerPictureRequest)
    {
        if (editMarkerPictureRequest == null)
        {
            return this.ApiError("Object can't be null");
        }

        var marker = await _markerService.GetByIdAsync(editMarkerPictureRequest.Id);
        if (marker == null)
        {
            throw new NotFoundException("Marker not found");
        }

        if (editMarkerPictureRequest.NewImageFile == null || editMarkerPictureRequest.NewImageFile.Length == 0)
        {
            return this.ApiError("ImageFile is required");
        }
        var urlImage = await _imageManagementMarkerServices.AddImageMarker(editMarkerPictureRequest.NewImageFile);
        if (urlImage == null)
        {
            return this.ApiError("Error uploading image");
        }
        await _imageManagementMarkerServices.DeleteImageMarker(marker.Image);

        var editMarker = await _markerService.AddMarkerImage(editMarkerPictureRequest.Id, urlImage);

        return this.ApiOk(editMarker);
    }

    [HttpDelete]
    [Route("[action]/{id}", Name = "DeleteMarker")]
    public async Task<IActionResult> DeleteMarker(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return this.ApiError("id can't be null or empty");
        }

        var marker = await _markerService.GetByIdAsync(id);
        if (marker == null)
        {
            throw new NotFoundException("Marker not found");
        }

        if (marker.Status == false)
        {
            return this.ApiError("Marker is already inactive");
        }

        marker.Status = false;
        await _markerService.UpdateAsync(marker);
        return CreatedAtRoute("GetMarkerById", new { id = marker.Id }, this.ApiOk(marker));
    }

    [HttpPut]
    [Route("RestoreMarker/{id}")]
    public async Task<IActionResult> RestoreMarker(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return this.ApiError("id can't be null or empty");
        }

        var marker = await _markerService.GetByIdAsync(id);
        if (marker == null)
        {
            throw new NotFoundException("Marker not found");
        }

        if (marker.Status == true)
        {
            return this.ApiError("Marker is already active");
        }

        marker.Status = true;
        await _markerService.UpdateAsync(marker);
        return this.ApiOk("Marker restored successfully");
    }
}
