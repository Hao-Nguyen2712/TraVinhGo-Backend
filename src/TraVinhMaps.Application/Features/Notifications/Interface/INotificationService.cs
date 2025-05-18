// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.Notifications.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Notifications.Interface;
public interface INotificationService
{
    Task<Notification> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> ListAsync(Expression<Func<Notification, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Notification> AddAsync(Notification entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> AddRangeAsync(IEnumerable<Notification> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Notification entity, CancellationToken cancellationToken = default);
    Task<long> CountAsync(Expression<Func<Notification, bool>> predicate = null, CancellationToken cancellationToken = default);
    Task<Notification> GetAsyns(Expression<Func<Notification, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a notification to all users based on the request.
    /// </summary>
    /// <param name="request">The notification request containing title, content, read status, and icon code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task indicating success.</returns>
    Task<bool> SendNotificationAsync(NotificationRequest notificationRequest, CancellationToken cancellation = default);
    Task<bool> MarkNotificationAsReadAsync(string notificationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId, bool? isRead = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetUniqueNotificationsAsync(CancellationToken cancellationToken = default);
}
