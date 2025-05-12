// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Application.Features.Destination.Interface;
using TraVinhMaps.Application.Features.Destination;
using TraVinhMaps.Application.Features.Destination.Models;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Application.Features.Destination.Mappers;
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
        return Ok(list);
    }

    [HttpGet]
    [Route("GetTouristDestinationPaging")]
    public async Task<IActionResult> GetTouristDestinationPaging([FromQuery]TouristDestinationSpecParams touristDestinationSpecParams)
    {
        var list = await this._touristDestinationService.GetTouristDestination(touristDestinationSpecParams);
        return Ok(list);
    }

    [HttpGet]
    [Route("GetDeletedDestination")]
    public async Task<IActionResult> GetDeletedDestination()
    {
        var list = await this._touristDestinationService.ListAsync(p => p.status == false);
        return Ok(list);
    }

    [HttpGet]
    [Route("GetActiveDestination")]
    public async Task<IActionResult> GetActiveDestination()
    {
        var list = await this._touristDestinationService.ListAsync(p => p.status == true);
        return Ok(list);
    }

    [HttpGet]
    [Route("GetCountDestination")]
    public async Task<IActionResult> GetCountDestination()
    {
        var count = await this._touristDestinationService.CountAsync();
        return Ok(count);
    }

    [HttpGet]
    [Route("[action]/{id}", Name = "GetDestinationById")]
    public async Task<IActionResult> GetDestinationById(string id)
    {
        if(id == null)
        {
            return BadRequest("id can't be null");
        }
        var destination = await this._touristDestinationService.GetByIdAsync(id);
        if(destination == null)
        {
            return NotFound();
        }
        return Ok(destination);
    }

    [HttpPost]
    [Route("CreateDestination")]
    public async Task<IActionResult> CreateDestination([FromForm] TouristDestinationRequest touristDestination)
    {
        List<String> linkHistoryImage= null;

        if (touristDestination.ImagesFile == null || touristDestination.ImagesFile.Count == 0)
        {
            return BadRequest("Tourist attractions must have at least 1 photo");
        }
        var linkImage = await _imageManagementDestinationServices.AddImageDestination(touristDestination.ImagesFile);

        if (linkImage == null) return BadRequest("No valid image uploaded.");

        if (touristDestination.HistoryStory.ImagesFile != null || touristDestination.HistoryStory.ImagesFile.Count > 0)
        {
            linkHistoryImage = await _imageManagementDestinationServices.AddImageDestination(touristDestination.HistoryStory.ImagesFile);
            if (linkHistoryImage == null) return BadRequest("No valid history image uploaded.");
        }
        var touristDestination1 = DestinationMapper.Mapper.Map<TouristDestination>(touristDestination);
        var newDestination = await this._touristDestinationService.AddAsync(touristDestination1);
        
        foreach (var item in linkImage)
        {
            await this._touristDestinationService.AddDestinationImage(newDestination.Id, item);
        }
        if(linkHistoryImage !=null)
        {
            foreach (var historyItem in linkHistoryImage)
            {
                await this._touristDestinationService.AddDestinationHistoryStoryImage(newDestination.Id, historyItem);
            }
        }
        return CreatedAtRoute("GetDestinationById", new {id = newDestination.Id }, newDestination);
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
        return Ok(linkImage);
    }
    [HttpPost]
    [Route("AddDestinationHistoryStoryImage")]
    public async Task<IActionResult> AddDestinationHistoryStoryImage ([FromForm] AddImageRequest addImageRequest)
    {
        var linkImage = await _imageManagementDestinationServices.AddImageDestination(addImageRequest.imageFile);
        foreach (var item in linkImage)
        {
            await this._touristDestinationService.AddDestinationHistoryStoryImage(addImageRequest.id, item);
        }
        return Ok(linkImage);
    }

    [HttpDelete]
    [Route("DeleteDestinationImage")]
    public async Task<IActionResult> DeleteDestinationImage([FromBody] DeleteDestinationImageRequest deleteDestinationImageRequest)
    {
        var isDeleteUrl = await this._imageManagementDestinationServices.DeleteImageDestination(deleteDestinationImageRequest.imageUrl);
        if(!isDeleteUrl)
        {
            return BadRequest("No valid images url were removed.");
        }
        var result = await this._touristDestinationService.DeleteDestinationImage(deleteDestinationImageRequest.id, deleteDestinationImageRequest.imageUrl);

        if (result == "Image deleted successfully")
        {
            return Ok(true);
        }
        return BadRequest("No valid images were removed.");
    }

    [HttpDelete]
    [Route("DeleteDestinationHistoryStoryImage")]
    public async Task<IActionResult> DeleteDestinationHistoryStoryImage ([FromBody] DeleteDestinationImageRequest deleteDestinationImageRequest)
    {
        var isDeleteUrl = await this._imageManagementDestinationServices.DeleteImageDestination(deleteDestinationImageRequest.imageUrl);
        if (!isDeleteUrl)
        {
            return BadRequest("No valid images url were removed.");
        }
        var result = await this._touristDestinationService.DeleteDestinationHistoryStoryImage(deleteDestinationImageRequest.id, deleteDestinationImageRequest.imageUrl);

        if (result == "Image deleted successfully")
        {
            return Ok(true);
        }
        return BadRequest("No valid images were removed.");
    }

    [HttpPut]
    [Route("UpdateDestination")]
    public async Task<IActionResult> UpdateDestination([FromBody]UpdateDestinationRequest updateDestinationRequest)
    {
        if (updateDestinationRequest == null)
        {
            return BadRequest("Object can't be null");
        }
        var destination = await _touristDestinationService.GetByIdAsync(updateDestinationRequest.Id);
        if (destination == null)
        {
            return NotFound();
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
        destination.UpdateAt = DateTime.Now;
        await this._touristDestinationService.UpdateAsync(destination);
        return CreatedAtRoute("GetDestinationById", new { id = updateDestinationRequest.Id }, updateDestinationRequest);
    }

    [HttpPut]
    [Route("PlusFavorite/{id}")]
    public async Task<IActionResult> PlusFavorite(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("id can't be null or empty");

        var result = await _touristDestinationService.PlusFavorite(id);

        if (result)
            return Ok("Favorite count increased successfully");
        return NotFound("Destination not found or update failed");
    }

    [HttpDelete]
    [Route("DeleteDestination/{id}")]
    public async Task<IActionResult> DeleteDestination(string id)
    {
        if (id == null)
        {
            return BadRequest("id can't be null");
        }
        var destination = await _touristDestinationService.GetByIdAsync(id);
        if (destination == null)
        {
            return NotFound();
        }
        destination.status = false;
        await this._touristDestinationService.UpdateAsync(destination);
        //return NoContent();
        return Ok("Destination deleted successfully");
    }
}
