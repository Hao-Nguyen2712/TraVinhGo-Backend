// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraVinhMaps.Application.Features.Destination.Models;
public class DestinationStatsOverview
{
    public long TotalDestinations { get; set; }
    public long TotalViews { get; set; }
    public long TotalInteractions { get; set; }
    public long TotalFavorites { get; set; }
    public List<DestinationAnalytics> DestinationDetails { get; set; }
}
