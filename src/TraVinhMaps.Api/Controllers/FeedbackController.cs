// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Api.Hubs;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.Feedback.Interface;
using TraVinhMaps.Application.Features.Feedback.Models;
using TraVinhMaps.Application.Features.Users.Interface;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;
    private readonly IHubContext<DashboardHub> _hubContext;
    private readonly IUserService _userService;
    public FeedbackController(IFeedbackService feedbackService, IHubContext<DashboardHub> hubContext, IUserService userService)
    {
        _feedbackService = feedbackService;
        _hubContext = hubContext;
        _userService = userService;
    }

    // GET endpoint to retrieve all feedbacks
    // Route: api/Feedback/all
    //[Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllFeedbacks()
    {
        var feedbacks = await _feedbackService.ListAllAsync();
        return this.ApiOk(feedbacks);
    }

    // GET endpoint to retrieve feedback by userId
    // Route: api/Feedback/{userId}
    // Name: Names the route for use in link generation
    [HttpGet("user/{id}", Name = "GetFeedBackByUserId")]
    public async Task<IActionResult> GetFeedBackByUserId(string id)
    {
        var feedback = await _feedbackService.GetByIdAsync(id);
        if (feedback == null)
        {
            throw new NotFoundException("Feedback by userId not found!");
        }
        return this.ApiOk(feedback);
    }

    // GET endpoint to retrieve feedback by feedbackId
    // Route: api/Feedback/{feedbackId}
    // Name: Names the route for use in link generation
    [HttpGet("details/{id}", Name = "GetFeedBackByFeedbackId")]
    public async Task<IActionResult> GetFeedBackByFeedbackId(string id)
    {
        var feedback = await _feedbackService.GetByIdAsync(id);
        if (feedback == null)
        {
            throw new NotFoundException("Feedback by userId not found!");
        }
        return this.ApiOk(feedback);
    }

    // POST endpoint to send feedback, requires authentication
    // Route: api/Feedback/send
    [Authorize]
    [HttpPost("send")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SendFeedback([FromForm] FeedbackRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var feedback = await _feedbackService.AddAsync(request);

            var user = await _userService.GetByIdAsync(feedback.UserId);
            var username = user != null ? user.Username : "Unknown";
            var feedbackPayload = new
            {
                id = feedback.Id,
                Content = feedback.Content,
                CreatedAt = feedback.CreatedAt,
                Username = username,
                UserId = feedback.UserId,
                Images = feedback.Images
            };

            // Get the SignalR hub context to notify admins
            await _hubContext.Clients.Group("admin").SendAsync("ReceiveFeedback", feedbackPayload);

            return this.ApiOk(feedback);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while processing feedback", Error = ex.Message });
        }
    }
}
