// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson;
using MongoDB.Driver;
using TraVinhMaps.Application.Features.Notifications.Models;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure.CustomRepositories;
public class NotificationsRepository : BaseRepository<Notification>, INotificationsRepository
{
    private readonly IMongoCollection<Notification> _notificationCollection;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    public NotificationsRepository(IDbContext context, IUserRepository userRepository, IRoleRepository roleRepository = null) : base(context)
    {
        _notificationCollection = context.GetCollection<Notification>();
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<IEnumerable<Notification>> GetUniqueNotificationsAsync(CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationCollection.Find(_ => true).ToListAsync(cancellationToken);
        return notifications
            .GroupBy(n => new { n.Title, n.Content })
            .Select(g => g.First())
            .OrderBy(n => n.CreatedAt).ToList();
    }

    public async Task<bool> SendNotificationAsync(NotificationRequest notificationRequest, CancellationToken cancellation)
    {
        string displayTitle = notificationRequest.IconCode.StartsWith("fa-")
            ? $"<i class='fas {notificationRequest.IconCode}'></i> {notificationRequest.Title}"
            : $"{GetEmojiFromCode(notificationRequest.IconCode)} {notificationRequest.Title ?? string.Empty}";

        var notification = new Notification
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = displayTitle,
            Content = notificationRequest.Content,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await _notificationCollection.InsertOneAsync(notification, null, cancellation);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private string GetEmojiFromCode(string code)
    {
        return code switch
        {
            "bun" => "\uD83C\uDF5C", // Emoji bat bun
            "moon" => "\uD83C\uDF1D", // Emoji the moon 
            "heart" => "\u2764\uFE0F", // Emoji the heart
            "sparkles" => "\u2728", // Emoji the star
            _ => "\uD83C\uDF5C" // Default  bat bun
        };
    }
}
