// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Application.Features.Admins.Models;
public class UpdateAdminSettingRequest
{
    public required string UpdateValue { get; set; }
    public required string UpdateType { get; set; } // e.g., "email", "phoneNumber",
}
