// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TraVinhMaps.Infrastructure.Hubs;
public class FeedbackHub : Hub
{
    public async Task SendFeedbackNotification(string feedbackId)
    {

        await Clients.Group("admin").SendAsync("ReceiveFeedback", feedbackId);
        await Clients.Group("super-admin").SendAsync("ReceiveFeedback", feedbackId);
    }

    public async Task JoinAdminGroup(string role)
    {

        if (role == "amin")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }
        else if (role == "super-admin")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SuperAdmins");
        }
    }
}
