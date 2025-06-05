// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraVinhMaps.Application.Features.SellingLink.Models;
public class CreateSellingLinkRequest
{
    public required string Tittle { get; set; }
    public required string Link { get; set; }
}
