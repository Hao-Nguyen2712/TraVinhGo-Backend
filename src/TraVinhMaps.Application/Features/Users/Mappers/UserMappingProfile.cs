// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TraVinhMaps.Application.Features.Users.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Users.Mappers;
public class UserMappingProfile : AutoMapper.Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserRequest, User>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ReverseMap();
    }
}
