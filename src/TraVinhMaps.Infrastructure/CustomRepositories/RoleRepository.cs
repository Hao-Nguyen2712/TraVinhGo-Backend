// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
