// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.Roles.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.UnitOfWorks;
public interface IRoleRepository : IRepository<Role>
{
    Task<Role> AddAsync(RoleRequest entity, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(string id, RoleRequest entity, CancellationToken cancellationToken = default);
}
