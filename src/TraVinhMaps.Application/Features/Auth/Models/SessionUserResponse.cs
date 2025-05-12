// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Application.Features.Auth.Models;

public class SessionUserResponse
{
    public string? UserId { get; set; }
    public string? DeviceInfo { get; set; }
    public string? IpAddress { get; set; }
}
