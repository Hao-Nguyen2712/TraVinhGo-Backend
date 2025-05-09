// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TraVinhMaps.Application.Features.Destination.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Destination.Mappers;
public class DestinationMappingProfile : AutoMapper.Profile
{
    public DestinationMappingProfile()
    {
        CreateMap<TouristDestinationRequest, TouristDestination>().ReverseMap();
    }
}
