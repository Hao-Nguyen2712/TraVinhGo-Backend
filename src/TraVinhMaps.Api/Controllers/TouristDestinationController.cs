// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.Destination;
using TraVinhMaps.Application.Features.Destination.Interface;
using TraVinhMaps.Application.Features.Destination.Mappers;
using TraVinhMaps.Application.Features.Destination.Models;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Domain.Specs;

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
    [Route("GetAllDestinations")]
    public async Task<IActionResult> GetAllDestinations()
    {
        var list = await this._touristDestinationService.ListAllAsync();
        return this.ApiOk(list);
    }

    [HttpGet]
    [Route("GetTouristDestinationPaging")]
    public async Task<IActionResult> GetTouristDestinationPaging([FromQuery] TouristDestinationSpecParams touristDestinationSpecParams)
    {
        var list = await this._touristDestinationService.GetTouristDestination(touristDestinationSpecParams);
        return this.ApiOk(list);
    }

    [HttpGet]
    [Route("GetDeletedDestination")]
    public async Task<IActionResult> GetDeletedDestination()
    {
        var list = await this._touristDestinationService.ListAsync(p => p.status == false);
        return this.ApiOk(list);
    }

    [HttpGet]
    [Route("GetActiveDestination")]
    public async Task<IActionResult> GetActiveDestination()
    {
        var list = await this._touristDestinationService.ListAsync(p => p.status == true);
        return this.ApiOk(list);
    }

    [HttpGet]
    [Route("GetCountDestination")]
    public async Task<IActionResult> GetCountDestination()
    {
        var count = await this._touristDestinationService.CountAsync();
        return this.ApiOk(count);
    }

    [HttpGet]
    [Route("[action]/{id}", Name = "GetDestinationById")]
    public async Task<IActionResult> GetDestinationById(string id)
    {
        if (id == null)
        {
            return BadRequest("id can't be null");
        }
        var destination = await this._touristDestinationService.GetByIdAsync(id);
        if (destination == null)
        {
            return NotFound();
        }
        return this.ApiOk(destination);
    }

    [HttpPost]
    [Route("CreateDestination")]
    public async Task<IActionResult> CreateDestination([FromForm] TouristDestinationRequest touristDestination)
    {
        List<String> linkHistoryImage = null;

        if (touristDestination.ImagesFile == null || touristDestination.ImagesFile.Count == 0)
        {
            return this.ApiError("Tourist attractions must have at least 1 photo");
        }
        var linkImage = await _imageManagementDestinationServices.AddImageDestination(touristDestination.ImagesFile);

        if (linkImage == null) return this.ApiError("No valid image uploaded.");

        if (touristDestination.HistoryStory != null &&
            touristDestination.HistoryStory.ImagesFile != null &&
            touristDestination.HistoryStory.ImagesFile.Count > 0)
        {
            linkHistoryImage = await _imageManagementDestinationServices.AddImageDestination(touristDestination.HistoryStory.ImagesFile);
            if (linkHistoryImage == null)
                return this.ApiError("No valid history image uploaded.");
        }

        var touristDestination1 = DestinationMapper.Mapper.Map<TouristDestination>(touristDestination);
        var newDestination = await this._touristDestinationService.AddAsync(touristDestination1);

        foreach (var item in linkImage)
        {
            await this._touristDestinationService.AddDestinationImage(newDestination.Id, item);
        }
        if (linkHistoryImage != null)
        {
            foreach (var historyItem in linkHistoryImage)
            {
                await this._touristDestinationService.AddDestinationHistoryStoryImage(newDestination.Id, historyItem);
            }
        }
        return CreatedAtRoute("GetDestinationById", new { id = newDestination.Id }, this.ApiOk(newDestination));
    }

    //[HttpPost]
    //[Route("AddDestinationImage")]
    //public async Task<IActionResult> AddDestinationImage([FromForm] AddImageRequest addImageRequest)
    //{
    //    var linkImage = await _imageManagementDestinationServices.AddImageDestination(addImageRequest.imageFile);
    //    return Ok(linkImage);
    //}

    [HttpPost]
    [Route("AddDestinationImage")]
    public async Task<IActionResult> AddDestinationImage([FromForm] AddImageRequest addImageRequest)
    {
        var linkImage = await _imageManagementDestinationServices.AddImageDestination(addImageRequest.imageFile);
        foreach (var item in linkImage)
        {
            await this._touristDestinationService.AddDestinationImage(addImageRequest.id, item);
        }
        return this.ApiOk(linkImage);
    }
    [HttpPost]
    [Route("AddDestinationHistoryStoryImage")]
    public async Task<IActionResult> AddDestinationHistoryStoryImage([FromForm] AddImageRequest addImageRequest)
    {
        var linkImage = await _imageManagementDestinationServices.AddImageDestination(addImageRequest.imageFile);
        foreach (var item in linkImage)
        {
            await this._touristDestinationService.AddDestinationHistoryStoryImage(addImageRequest.id, item);
        }
        return this.ApiOk(linkImage);
    }

    [HttpPost]
    [Route("DeleteDestinationImage")]
    public async Task<IActionResult> DeleteDestinationImage([FromBody] DeleteDestinationImageRequest deleteDestinationImageRequest)
    {
        var isDeleteUrl = await this._imageManagementDestinationServices.DeleteImageDestination(deleteDestinationImageRequest.imageUrl);
        if (!isDeleteUrl)
        {
            return this.ApiError("No valid images url were removed.");
        }
        var result = await this._touristDestinationService.DeleteDestinationImage(deleteDestinationImageRequest.id, deleteDestinationImageRequest.imageUrl);

        if (result == "Image deleted successfully")
        {
            return this.ApiOk(true);
        }
        return this.ApiError("No valid images were removed.");
    }

    [HttpPost]
    [Route("DeleteDestinationHistoryStoryImage")]
    public async Task<IActionResult> DeleteDestinationHistoryStoryImage([FromBody] DeleteDestinationImageRequest deleteDestinationImageRequest)
    {
        var isDeleteUrl = await this._imageManagementDestinationServices.DeleteImageDestination(deleteDestinationImageRequest.imageUrl);
        if (!isDeleteUrl)
        {
            return this.ApiError("No valid images url were removed.");
        }
        var result = await this._touristDestinationService.DeleteDestinationHistoryStoryImage(deleteDestinationImageRequest.id, deleteDestinationImageRequest.imageUrl);

        if (result == "Image deleted successfully")
        {
            return this.ApiOk(true);
        }
        return this.ApiError("No valid images were removed.");
    }

    [HttpPut]
    [Route("UpdateDestination")]
    public async Task<IActionResult> UpdateDestination([FromBody] UpdateDestinationRequest updateDestinationRequest)
    {
        if (updateDestinationRequest == null)
        {
            return this.ApiError("Object can't be null");
        }
        var destination = await _touristDestinationService.GetByIdAsync(updateDestinationRequest.Id);
        if (destination == null)
        {
            throw new NotFoundException("No Destination was found");
        }
        if (destination.HistoryStory == null)
        {
            destination.HistoryStory = new HistoryStory();
        }
        destination.Name = updateDestinationRequest.Name;
        destination.Description = updateDestinationRequest.Description;
        destination.AvarageRating = updateDestinationRequest.AvarageRating;
        destination.Address = updateDestinationRequest.Address;
        destination.Location = updateDestinationRequest.Location;
        destination.HistoryStory.Content = updateDestinationRequest.HistoryStory.Content;
        destination.DestinationTypeId = updateDestinationRequest.DestinationTypeId;
        destination.OpeningHours = updateDestinationRequest.OpeningHours;
        destination.Capacity = updateDestinationRequest.Capacity;
        destination.Contact = updateDestinationRequest.Contact;
        destination.TagId = updateDestinationRequest.TagId;
        destination.Ticket = updateDestinationRequest.Ticket;
        //destination.TicketCount = updateDestinationRequest.TicketCount;
        destination.UpdateAt = DateTime.Now.ToLocalTime();
        await this._touristDestinationService.UpdateAsync(destination);
        return CreatedAtRoute("GetDestinationById", new { id = updateDestinationRequest.Id }, this.ApiOk(updateDestinationRequest));
    }

    [HttpPut]
    [Route("PlusFavorite/{id}")]
    public async Task<IActionResult> PlusFavorite(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return this.ApiError("id can't be null or empty");

        var result = await _touristDestinationService.PlusFavorite(id);

        if (result)
            return this.ApiOk("Favorite count increased successfully");
        throw new NotFoundException("Destination not found or update failed");
    }

    [HttpDelete]
    [Route("DeleteDestination/{id}")]
    public async Task<IActionResult> DeleteDestination(string id)
    {
        if (id == null)
        {
            return this.ApiError("id can't be null");
        }
        var destination = await _touristDestinationService.GetByIdAsync(id);
        if (destination == null)
        {
            throw new NotFoundException("No Destination was found");
        }
        if (destination.status == false)
        {
            return this.ApiError("Destination is already inactive");
        }
        destination.status = false;
        destination.UpdateAt = DateTime.Now.ToLocalTime();
        await this._touristDestinationService.UpdateAsync(destination);
        //return NoContent();
        return this.ApiOk("Destination deleted successfully");
    }

    [HttpPut]
    [Route("RestoreDestination/{id}")]
    public async Task<IActionResult> RestoreDestination(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return this.ApiError("id can't be null or empty");
        }

        var destination = await _touristDestinationService.GetByIdAsync(id);
        if (destination == null)
        {
            throw new NotFoundException("No Destination was found");
        }

        if (destination.status == true)
        {
            return this.ApiError("Destination is already active");
        }

        destination.status = true;
        destination.UpdateAt = DateTime.Now.ToLocalTime();

        await _touristDestinationService.UpdateAsync(destination);
        return this.ApiOk("Destination restored successfully");
    }


    // Analytics
    // Format date: yyyy-mm-dd

    [HttpGet("stats-overview")]
    public async Task<IActionResult> GetStatsOverview([FromQuery] string timeRange = "month", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var stats = await _touristDestinationService.GetDestinationStatsOverviewAsync(timeRange, startDate, endDate);
            if (stats == null)
                throw new NotFoundException("No analytics data available.");
            return this.ApiOk(stats);
        }
        catch (Exception ex)
        {
            var errorDetails = new
            {
                message = ex.Message,
                stackTrace = ex.StackTrace,
                innerException = ex.InnerException?.ToString(),
                timeRange,
                startDate,
                endDate
            };
            return StatusCode(500, errorDetails);
        }
    }

    // Top 5 Favorite Destinations
    [HttpGet("stats-getTopFavoritesDestinations")]
    public async Task<IActionResult> GetTopDestinationsByFavorites(int top = 5, string timeRange = "month", DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var getTop = await _touristDestinationService.GetTopDestinationsByFavoritesAsync(5, timeRange, startDate, endDate);
            if (getTop == null)
                throw new NotFoundException("No analytics data available.");
            return this.ApiOk(getTop);
        }
        catch (Exception ex)
        {
            var errorDetails = new
            {
                message = ex.Message,
                stackTrace = ex.StackTrace,
                innerException = ex.InnerException?.ToString(),
                timeRange,
                startDate,
                endDate
            };
            return StatusCode(500, errorDetails);
        }
    }

    // Top 5 View Destinations
    [HttpGet("stats-getTopViewsDestinations")]
    public async Task<IActionResult> GetTopDestinationsByViews(int top = 5, string timeRange = "month", DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var getTop = await _touristDestinationService.GetTopDestinationsByViewsAsync(5, timeRange, startDate, endDate);
            if (getTop == null)
                throw new NotFoundException("No analytics data available.");
            return this.ApiOk(getTop);
        }
        catch (Exception ex)
        {
            var errorDetails = new
            {
                message = ex.Message,
                stackTrace = ex.StackTrace,
                innerException = ex.InnerException?.ToString(),
                timeRange,
                startDate,
                endDate
            };
            return StatusCode(500, errorDetails);
        }
    }

    // UserDemographicsAsync
    [HttpGet("stats-getUserDemographics")]
    public async Task<IActionResult> GetUserDemographics(string timeRange = "month", DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var getTop = await _touristDestinationService.GetUserDemographicsAsync(timeRange, startDate, endDate);
            if (getTop == null)
                throw new NotFoundException("No analytics data available.");
            return this.ApiOk(getTop);
        }
        catch (Exception ex)
        {
            var errorDetails = new
            {
                message = ex.Message,
                stackTrace = ex.StackTrace,
                innerException = ex.InnerException?.ToString(),
                timeRange,
                startDate,
                endDate
            };
            return StatusCode(500, errorDetails);
        }
    }

    // Compare
    [HttpGet("stats-compare")]
    public async Task<IActionResult> CompareDestinations([FromQuery] IEnumerable<string> destinationIds, [FromQuery] string timeRange = "month", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var getTop = await _touristDestinationService.CompareDestinationsAsync(destinationIds, timeRange, startDate, endDate);
            if (getTop == null)
                throw new NotFoundException("No analytics data available.");
            return this.ApiOk(getTop);
        }
        catch (Exception ex)
        {
            var errorDetails = new
            {
                message = ex.Message,
                stackTrace = ex.StackTrace,
                innerException = ex.InnerException?.ToString(),
                timeRange,
                startDate,
                endDate
            };
            return StatusCode(500, errorDetails);
        }
    }

    [HttpGet("top-favorite-destination")]
    public async Task<IActionResult> GetTop10FavoriteDestination()
    {
        var result = await _touristDestinationService.GetTop10FavoriteDestination();
        if (result == null || !result.Any())
        {
            this.ApiError("No favorite destinations found.");
        }
        return this.ApiOk(result);
    }

}
