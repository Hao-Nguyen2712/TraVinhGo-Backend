// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace TraVinhMaps.Application.Features.Destination.Mappers;
public static class DestinationMapper
{
    private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
            cfg.AddProfile<DestinationMappingProfile>();
        });
        var mapper = config.CreateMapper();
        return mapper;
    });

    public static IMapper Mapper => lazy.Value;
}
