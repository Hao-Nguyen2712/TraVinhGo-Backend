// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Users.Models;

public class UserRequest
{
    public string Id { get; set; } = default!;
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Password { get; set; }
    public string? Email { get; set; }
    public required string RoleId { get; set; }
    public Profile? Profile { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<Favorite>? Favorites { get; set; }
    public bool IsForbidden { get; set; }
    public bool Status { get; set; }
}
