// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Application.Features.SellingLink.Models;
public class UpdateSellingLinkRequest
{
    public required string Id { get; set; }
    public required string ProductId { get; set; }
    public required string Title { get; set; }
    public required string Link { get; set; }
    public DateTime? UpdateAt { get; set; } = DateTime.UtcNow;
}
