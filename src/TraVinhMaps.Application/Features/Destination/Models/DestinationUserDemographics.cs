// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraVinhMaps.Application.Features.Destination.Models;
public class DestinationUserDemographics
{
    public string Id { get; set; }
    public string LocationName { get; set; }
    public string AgeGroup { get; set; }
    public string Hometown { get; set; }
    public long UserCount { get; set; }
}
