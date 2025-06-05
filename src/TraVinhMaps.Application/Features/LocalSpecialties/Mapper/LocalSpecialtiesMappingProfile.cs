// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TraVinhMaps.Application.Features.LocalSpecialties.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.LocalSpecialties.Mapper;
public class LocalSpecialtiesMappingProfile : AutoMapper.Profile
{
    public LocalSpecialtiesMappingProfile()
    {
        // Tạo mới đặc sản (KHÔNG bao gồm location)
        CreateMap<CreateLocalSpecialtiesRequest, Domain.Entities.LocalSpecialties>();

        // Update đặc sản (CÓ bao gồm location từ AddLocationRequest)
        CreateMap<UpdateLocalSpecialtiesRequest, Domain.Entities.LocalSpecialties>();

        // Map từ AddLocationRequest sang LocalSpecialtyLocation
        CreateMap<AddLocationRequest, LocalSpecialtyLocation>()
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location));

        // Map từ LocationRequest sang Location
        CreateMap<LocationRequest, Location>();

        // Reverse 
        CreateMap<Domain.Entities.LocalSpecialties, CreateLocalSpecialtiesRequest>();
        CreateMap<Domain.Entities.LocalSpecialties, UpdateLocalSpecialtiesRequest>();

        CreateMap<LocalSpecialtyLocation, AddLocationRequest>();
        CreateMap<Location, LocationRequest>();
    }
}
