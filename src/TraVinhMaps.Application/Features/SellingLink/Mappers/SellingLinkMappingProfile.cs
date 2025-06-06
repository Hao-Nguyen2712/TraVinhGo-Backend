// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AutoMapper;
using TraVinhMaps.Application.Features.SellingLink.Models;

namespace TraVinhMaps.Application.Features.SellingLink.Mappers;
public class SellingLinkMappingProfile : Profile
{
    public SellingLinkMappingProfile()
    {
        CreateMap<Domain.Entities.SellingLink, CreateSellingLinkRequest>().ReverseMap();
        CreateMap<Domain.Entities.SellingLink, UpdateSellingLinkRequest>().ReverseMap();
    }
}
