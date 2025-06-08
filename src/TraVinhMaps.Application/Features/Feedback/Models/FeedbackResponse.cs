// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraVinhMaps.Application.Features.Feedback.Models;
public class FeedbackResponse
{
    public string Id { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Content { get; set; } = default!;
    public List<string>? Images { get; set; }
    public DateTime CreatedAt { get; set; }
}
