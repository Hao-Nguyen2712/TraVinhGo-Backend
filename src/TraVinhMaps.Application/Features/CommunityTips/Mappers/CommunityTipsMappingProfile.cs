// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TraVinhMaps.Application.Features.CommunityTips.Models;
using TraVinhMaps.Application.Features.OcopProduct.Models;

namespace TraVinhMaps.Application.Features.CommunityTips.Mappers;
public class CommunityTipsMappingProfile : Profile
{
    public CommunityTipsMappingProfile()
    {
        CreateMap<Domain.Entities.Tips, CreateCommunityTipRequest>().ReverseMap();
        CreateMap<Domain.Entities.Tips, UpdateCommunityTipRequest>().ReverseMap();
    }
}
