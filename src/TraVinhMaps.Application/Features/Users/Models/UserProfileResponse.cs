// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Application.Features.Users.Models;
public class UserProfileResponse
{
    public string? Fullname { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Gender { get; set; }
    public string? Avatar { get; set; }
    public string? Address { get; set; }
    public string? HassedPassword { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}
