// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.LocalSpecialties.Models;
public class AddLocationRequest
{
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string MarkerId { get; set; } = default!;
    public LocationRequest Location { get; set; } = default!;

    public LocalSpecialtyLocation ToLocationModel() => new LocalSpecialtyLocation
    {
        Name = this.Name,
        Address = this.Address,
        MarkerId = this.MarkerId,
        Location = new Location
        {
            Type = this.Location.Type,
            Coordinates = this.Location.Coordinates
        }
    };
}

public class LocationRequest
{
    public string Type { get; set; } = "Point"; 
    public List<double> Coordinates { get; set; } = default!; // [longitude, latitude]
}
