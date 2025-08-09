// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.Notifications.Interface;
using TraVinhMaps.Application.Features.Notifications.Models;

namespace TraVinhMaps.Api.Controllers;

// Controller for handling notification-related API endpoints
[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IFirebaseNotificationService _firebaseNotificationService;

    // Constructor - injects the NotificationService
    public NotificationsController(INotificationService notificationService, IFirebaseNotificationService firebaseNotificationService)
    {
        _notificationService = notificationService;
        _firebaseNotificationService = firebaseNotificationService;
    }

    // GET: api/Notifications/all
    // Retrieves a list of all notifications in the system
    [HttpGet("all")]
    public async Task<IActionResult> ListAllNotifications()
    {
        var notifications = await _notificationService.ListAllAsync();
        return Ok(notifications);
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentNotifications(CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationService.GetRecentNotificationsAsync(cancellationToken);
        return this.ApiOk(notifications);
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
    public async Task<ActionResult<IEnumerable<Domain.Entities.Notification>>> GetUniqueNotifications(CancellationToken cancellationToken = default)
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
        if (!result)
        {
            throw new BadRequestException("Failed to send notification.");
        }

        var response = await _firebaseNotificationService.PushNotificationAsync(request);

        return result
            ? this.ApiOk("Send Notification successfully!")
            : throw new BadRequestException("Failed to send notification.");
    }

}
