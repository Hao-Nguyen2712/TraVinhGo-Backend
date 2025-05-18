// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Application.Features.CommunityTips.Models;
public class UpdateCommunityTipRequest
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string TagId { get; set; }
    public required bool Status { get; set; }
    public DateTime? UpdateAt { get; set; } = DateTime.UtcNow;
}
