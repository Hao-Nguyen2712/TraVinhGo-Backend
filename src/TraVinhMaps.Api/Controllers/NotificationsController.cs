// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Api.Extensions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.Notifications.Interface;
using TraVinhMaps.Application.Features.Notifications.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> ListAllNotifications()
    {
        var notifications = await _notificationService.ListAllAsync();
        return Ok(notifications);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(string id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationService.GetByIdAsync(id, cancellationToken);
        return Ok(notification);
    }

    [HttpGet("unique")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetUniqueNotifications(CancellationToken cancellationToken = default)
    {
        var notifications =  await _notificationService.GetUniqueNotificationsAsync(cancellationToken);
        return Ok(notifications);
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request, CancellationToken cancellationToken)
    {
        var result = await _notificationService.SendNotificationAsync(request, cancellationToken);

        return result ? this.ApiOk("Send Notification successfully!") : throw new BadRequestException("Failed to send notification.");
    }

    [HttpPost("mark-as-read")]
    public async Task<IActionResult> MarkNotificationAsRead([FromBody] string notificationId, CancellationToken cancellationToken= default)
    {
        var result = await _notificationService.MarkNotificationAsReadAsync(notificationId, cancellationToken);
        return result ? this.ApiOk() : throw new BadRequestException("Failed to mark notification as read.");
    }

    [HttpGet("usser/{userId}")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationsByUserId(string userId, [FromQuery] bool? isRead = null, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId, isRead, cancellationToken);
        return Ok(notifications);
    }

}
