// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TraVinhMaps.Application.Features.Company.Models;

namespace TraVinhMaps.Application.Features.Company.Mappers;
public class CompanyMappingProfile : Profile
{
    public CompanyMappingProfile()
    {
        CreateMap<Domain.Entities.Company, CreateCompanyRequest>().ReverseMap();
        CreateMap<Domain.Entities.Company, UpdateCompanyRequest>().ReverseMap();
    }
}
