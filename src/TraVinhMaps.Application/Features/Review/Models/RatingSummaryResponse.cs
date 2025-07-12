// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraVinhMaps.Application.Features.Review.Models;
public  class RatingSummaryResponse
{
    public int OneStar { get; set; }
    public int TwoStar { get; set; }
    public int ThreeStar { get; set; }
    public int FourStar { get; set; }
    public int FiveStar { get; set; }

    public double OneStarPercent { get; set; }
    public double TwoStarPercent { get; set; }
    public double ThreeStarPercent { get; set; }
    public double FourStarPercent { get; set; }
    public double FiveStarPercent { get; set; }
}
