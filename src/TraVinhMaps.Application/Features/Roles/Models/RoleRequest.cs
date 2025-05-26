// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Application.Features.Roles.Models;
public class RoleRequest
{
    public required string Id { get; set; }
    public required string RoleName { get; set; }
    public required bool RoleStatus { get; set; }
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
