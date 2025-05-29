// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace TraVinhMaps.Application.Features.Admins.Models;
public class AdminRequest
{
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    public string RoleId { get; set; }
}
