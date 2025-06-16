// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.SignalR;

namespace TraVinhMaps.Api.Hubs;

public class DashboardHub : Hub
{
    // Feedback notification
    public async Task SendFeedbackNotification(string feedbackId)
    {
        await Clients.Group("admin").SendAsync("ReceiveFeedback", feedbackId);
        await Clients.Group("super-admin").SendAsync("ReceiveFeedback", feedbackId);
    }

    // User stats realtime update
    public async Task SendUserStatsUpdate()
    {
        await Clients.Group("admin").SendAsync("UpdateUserStats");
        await Clients.Group("super-admin").SendAsync("UpdateUserStats");
    }

    // Revenue realtime update
    public async Task SendRevenueUpdate()
    {
        await Clients.Group("admin").SendAsync("UpdateRevenueStats");
        await Clients.Group("super-admin").SendAsync("UpdateRevenueStats");
    }

    // Notification for dashboard
    public async Task SendGeneralNotification(string message)
    {
        await Clients.Group("admin").SendAsync("ReceiveGeneralNotification", message);
        await Clients.Group("super-admin").SendAsync("ReceiveGeneralNotification", message);
    }

    // Group join logic
    public async Task JoinAdminGroup(string role)
    {
        if (role == "admin")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "admin");
        }
        else if (role == "super-admin")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "super-admin");
        }
    }
}
