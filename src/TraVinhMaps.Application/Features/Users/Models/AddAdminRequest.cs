// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraVinhMaps.Application.Features.Users.Models;
public class AddAdminRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string RoleId { get; set; } = default!;
}
