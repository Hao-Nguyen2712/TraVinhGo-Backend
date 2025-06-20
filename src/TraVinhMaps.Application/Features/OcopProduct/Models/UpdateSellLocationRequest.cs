// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.OcopProduct.Models;
public class UpdateSellLocationRequest
{
    public string Id { get; set; }
    public string? LocationName { get; set; }
    public string? LocationAddress { get; set; }
    public string? MarkerId { get; set; }
    public Location? Location { get; set; }
}
