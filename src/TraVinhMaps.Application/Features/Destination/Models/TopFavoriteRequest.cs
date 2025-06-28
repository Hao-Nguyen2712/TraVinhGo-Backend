// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Application.Features.Destination.Models;
public record TopFavoriteRequest
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Image { get; set; }
    public double? AverageRating { get; set; }
    public string? Description { get; set; }
}
