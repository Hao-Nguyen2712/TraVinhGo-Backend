// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.EventAndFestivalFeature.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.EventAndFestivalFeature.Mappers;
public class EventAndFestivalMappingProfile : AutoMapper.Profile
{
    public EventAndFestivalMappingProfile()
    {
        CreateMap<CreateEventAndFestivalRequest, EventAndFestival>().ReverseMap();
    }
}
