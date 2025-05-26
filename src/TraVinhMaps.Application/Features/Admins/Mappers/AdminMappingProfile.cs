// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.Admins.Models;
using TraVinhMaps.Application.Features.Users.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Admins.Mappers;
public class AdminMappingProfile : AutoMapper.Profile
{
    public AdminMappingProfile()
    {
        CreateMap<AdminRequest, User>().ReverseMap();
        CreateMap<UpdateAdminRequest, User>().ReverseMap();
    }
}
