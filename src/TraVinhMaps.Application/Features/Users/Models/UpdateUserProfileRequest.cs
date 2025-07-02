// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Application.Features.Users.Models;
public class UpdateUserProfileRequest
{
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Gender { get; set; }
    public string? DateOfBirth { get; set; }
}
