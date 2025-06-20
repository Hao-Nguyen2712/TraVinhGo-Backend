// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TraVinhMaps.Application.Features.Notifications.Interface;
using TraVinhMaps.Application.Features.Notifications.Models;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Notifications;
public class NotificationsService : INotificationService
{
    private readonly ILogger<NotificationsService> _logger;
    private readonly INotificationsRepository _notificationsRepository;


    public NotificationsService(INotificationsRepository notificationsRepository, ILogger<NotificationsService> logger = null)
    {
        _notificationsRepository = notificationsRepository;
        _logger = logger;
    }

    public async Task<Notification> AddAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        return await _notificationsRepository.AddAsync(entity, cancellationToken);
    }

    public async Task<IEnumerable<Notification>> AddRangeAsync(IEnumerable<Notification> entities, CancellationToken cancellationToken = default)
    {
        return await _notificationsRepository.AddRangeAsync(entities, cancellationToken);
    }

    public async Task<long> CountAsync(Expression<Func<Notification, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return await _notificationsRepository.CountAsync(predicate, cancellationToken);
    }

    public async Task DeleteAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        await _notificationsRepository.DeleteAsync(entity, cancellationToken);
    }

    public async Task<Notification> GetAsyns(Expression<Func<Notification, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _notificationsRepository.GetAsyns(predicate, cancellationToken);
    }

    public async Task<Notification> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _notificationsRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Notification>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _notificationsRepository.ListAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<Notification>> ListAsync(Expression<Func<Notification, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _notificationsRepository.ListAsync(predicate, cancellationToken);
    }

    public async Task UpdateAsync(Notification entity, CancellationToken cancellationToken = default)
    {
        await _notificationsRepository.UpdateAsync(entity, cancellationToken);
    }
    public async Task<bool> SendNotificationAsync(NotificationRequest notificationRequest, CancellationToken cancellation)
    {
        return await _notificationsRepository.SendNotificationAsync(notificationRequest, cancellation);
    }

    public async Task<IEnumerable<Notification>> GetUniqueNotificationsAsync(CancellationToken cancellationToken = default)
    {
        return await _notificationsRepository.GetUniqueNotificationsAsync(cancellationToken);
    }
}
