// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.Notifications.Models;

namespace TraVinhMaps.Application.Features.Notifications.Interface;
public interface IFirebaseNotificationService
{
    Task<string> PushNotificationAsync(NotificationRequest notificationRequest, string topic = "all");
}
