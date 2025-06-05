// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TraVinhMaps.Application.Features.LocalSpecialties.Models;
public class UpdateLocalSpecialtiesRequest
{
    public string Id { get; set; } = default!;
    public string FoodName { get; set; } = default!;
    public string? Description { get; set; }
    public string TagId { get; set; } = default!;
    public bool Status { get; set; }
    public DateTime? UpdateAt { get; set; } = DateTime.UtcNow;
}
