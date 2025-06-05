// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TraVinhMaps.Application.Features.ItineraryPlan.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.ItineraryPlan.Mappers;
public class ItineraryPlanMappingProfile : AutoMapper.Profile
{
    public ItineraryPlanMappingProfile()
    {
        CreateMap<ItineraryPlanRequest, TraVinhMaps.Domain.Entities.ItineraryPlan>();
    }
}
