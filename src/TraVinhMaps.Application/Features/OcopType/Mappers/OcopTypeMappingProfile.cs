// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TraVinhMaps.Application.Features.OcopProduct.Models;
using TraVinhMaps.Application.Features.OcopType.Models;

namespace TraVinhMaps.Application.Features.OcopType.Mappers;
public class OcopTypeMappingProfile : Profile
{
    public OcopTypeMappingProfile()
    {
        CreateMap<Domain.Entities.OcopType, CreateOcopTypeRequest>().ReverseMap();
        CreateMap<Domain.Entities.OcopType, UpdateOcopTypeRequest>().ReverseMap();
    }
}
