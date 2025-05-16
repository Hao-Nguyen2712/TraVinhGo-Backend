// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TraVinhMaps.Application.Features.OcopProduct.Models;

namespace TraVinhMaps.Application.Features.OcopProduct.Mappers;
public class OcopProductMappingProfile : Profile
{
    public OcopProductMappingProfile()
    {
        CreateMap<Domain.Entities.OcopProduct, CreateOcopProductRequest>().ReverseMap();
        CreateMap<Domain.Entities.OcopProduct, UpdateOcopProductRequest>().ReverseMap();
    }
}
