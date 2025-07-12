// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraVinhMaps.Application.Features.Review.Models;
public class ReviewMobileListResponse
{
    public bool hasReviewed { get; set; }
    public IEnumerable<ReviewMobileResponse> Reviews { get; set; }
    public RatingSummaryResponse RatingSummary { get; set; }
}
