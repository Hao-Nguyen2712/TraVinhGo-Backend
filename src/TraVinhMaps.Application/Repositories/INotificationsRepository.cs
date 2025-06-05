// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TraVinhMaps.Application.Features.Notifications.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.UnitOfWorks;
public interface INotificationsRepository : IBaseRepository<Notification>
{
    Task<bool> SendNotificationAsync(NotificationRequest notificationRequest, CancellationToken cancellation = default);
    Task<bool> MarkNotificationAsReadAsync(string notificationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId, bool? isRead = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetUniqueNotificationsAsync(CancellationToken cancellationToken = default);
}
