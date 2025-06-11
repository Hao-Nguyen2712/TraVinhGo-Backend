// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.LocalSpecialties.Models;
public class UpdateLocationRequest
{
    public string LocationId { get; set; }
    public string? MarkerId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public LocationRequest Location { get; set; }

    public LocalSpecialtyLocation ToLocationModel() =>
        new LocalSpecialtyLocation
        {
            LocationId = this.LocationId,
            MarkerId = this.MarkerId,
            Name = this.Name,
            Address = this.Address,
            Location = new Location{
                Type = this.Location.Type,
                Coordinates = this.Location.Coordinates
            }
        };
}
