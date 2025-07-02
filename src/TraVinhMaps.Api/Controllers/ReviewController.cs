// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Api.Hubs;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.OcopProduct.Mappers;
using TraVinhMaps.Application.Features.OcopProduct.Models;
using TraVinhMaps.Application.Features.Review;
using TraVinhMaps.Application.Features.Review.Interface;
using TraVinhMaps.Application.Features.Review.Mappers;
using TraVinhMaps.Application.Features.Review.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly IHubContext<DashboardHub> _hubContext;
    public ReviewController(IReviewService reviewService, IHubContext<DashboardHub> hubContext)
    {
        _reviewService = reviewService;
        _hubContext = hubContext;
    }
    [HttpGet]
    [Route("GetAllReview")]
    public async Task<IActionResult> GetAllReview()
    {
        var listReview = await _reviewService.ListAllAsync();
        return this.ApiOk(listReview);
    }
    [HttpGet]
    [Route("GetReviewById/{id}", Name = "GetReviewById")]
    public async Task<IActionResult> GetReviewById(string id)
    {
        var review = await _reviewService.GetByIdAsync(id);
        return this.ApiOk(review);
    }
    [Authorize]
    [HttpGet]
    [Route("GetReviewByUserId", Name = "GetReviewByUserId")]
    public async Task<IActionResult> GetReviewByUserId(int rating, string destinationTypeId)
    {
        var review = await _reviewService.GetReviewsAsync(rating, destinationTypeId);
        return this.ApiOk(review);
    }
    [HttpGet]
    [Route("CountReviews")]
    public async Task<IActionResult> CountReviews()
    {
        var countReviews = await _reviewService.CountAsync();
        return this.ApiOk(countReviews);
    }
    [Authorize]
    [HttpPost("AddReview")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddReview([FromForm] CreateReviewRequest createReviewRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var review = await _reviewService.AddAsync(createReviewRequest);
            await _hubContext.Clients.Group("admin").SendAsync("ReceiveFeedback", review.Id);
            await _hubContext.Clients.Group("super-admin").SendAsync("ReceiveFeedback", review.Id);

            return this.ApiOk(review);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while processing review", Error = ex.Message });
        }
    }
    [HttpDelete]
    [Route("DeleteReview/{id}")]
    public async Task<IActionResult> DeleteReview(string id)
    {
        var review = await _reviewService.GetByIdAsync(id);
        if (review == null)
        {
            throw new NotFoundException("Review not found.");
        }
        await _reviewService.DeleteAsync(review);
        return this.ApiOk("Review deleted successfully.");
    }
    [Authorize]
    [HttpPost("AddReply")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddReply([FromForm] CreateReplyRequest replyRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var review = await _reviewService.GetByIdAsync(replyRequest.Id);
            if (review == null)
                throw new NotFoundException("Review not found.");

            var addedReply = await _reviewService.AddReply(replyRequest.Id, replyRequest);

            await _hubContext.Clients.Group("admin").SendAsync("ReceiveReply", replyRequest.Id);
            await _hubContext.Clients.Group("super-admin").SendAsync("ReceiveReply", replyRequest.Id);

            return this.ApiOk(addedReply);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while processing reply", Error = ex.Message });
        }
    }
}
