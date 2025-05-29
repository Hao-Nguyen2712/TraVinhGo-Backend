// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.Notifications.Interface;
using TraVinhMaps.Application.Features.Notifications.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Api.Controllers;

// Controller for handling notification-related API endpoints
[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    // Constructor - injects the NotificationService
    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    // GET: api/Notifications/all
    // Retrieves a list of all notifications in the system
    [HttpGet("all")]
    public async Task<IActionResult> ListAllNotifications()
    {
        var notifications = await _notificationService.ListAllAsync();
        return Ok(notifications);
    }

    // GET: api/Notifications/{id}
    // Retrieves a specific notification by its ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(string id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationService.GetByIdAsync(id, cancellationToken);
        return Ok(notification);
    }

    // GET: api/Notifications/unique
    // Retrieves only unique notifications (e.g., deduplicated list)
    [HttpGet("unique")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetUniqueNotifications(CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationService.GetUniqueNotificationsAsync(cancellationToken);
        return Ok(notifications);
    }

    // POST: api/Notifications/send
    // Sends a new notification based on the provided request payload
    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request, CancellationToken cancellationToken)
    {
        var result = await _notificationService.SendNotificationAsync(request, cancellationToken);
        return result
            ? this.ApiOk("Send Notification successfully!")
            : throw new BadRequestException("Failed to send notification.");
    }

    // POST: api/Notifications/mark-as-read
    // Marks a specific notification as read by its ID
    [HttpPost("mark-as-read")]
    public async Task<IActionResult> MarkNotificationAsRead([FromBody] string notificationId, CancellationToken cancellationToken = default)
    {
        var result = await _notificationService.MarkNotificationAsReadAsync(notificationId, cancellationToken);
        return result
            ? this.ApiOk()
            : throw new BadRequestException("Failed to mark notification as read.");
    }

    // GET: api/Notifications/usser/{userId}?isRead=true/false
    // Retrieves notifications for a specific user, optionally filtered by read status
    [HttpGet("usser/{userId}")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationsByUserId(string userId, [FromQuery] bool? isRead = null, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId, isRead, cancellationToken);
        return Ok(notifications);
    }
}
