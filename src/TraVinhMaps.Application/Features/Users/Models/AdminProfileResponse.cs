// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Application.Features.Users.Models;
public class AdminProfileResponse
{
    public string Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
    public string? Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? RoleName { get; set; }
    public bool IsForbidden { get; set; }
    public bool Status { get; set; }
}
