// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraVinhMaps.Application.Features.Notifications.Models;
public class NotificationRequest
{
    //public required string UserId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; } = false;
    public string IconCode { get; set; }

}
