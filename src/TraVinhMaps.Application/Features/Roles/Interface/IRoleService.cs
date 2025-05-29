// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Roles.Interface;
public interface IRoleService
{
    Task<Role> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> ListAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default);
}
