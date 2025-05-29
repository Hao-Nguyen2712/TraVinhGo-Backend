// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TraVinhMaps.Application.Features.Roles.Models;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;
using TraVinhMaps.Infrastructure.UnitOfWork;

namespace TraVinhMaps.Infrastructure.CustomRepositories;
public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(IDbContext context) : base(context)
    {
    }
}
