// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using TraVinhMaps.Application.Features.Roles.Interface;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Roles;
public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }
    public async Task<Role> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _roleRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Role>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _roleRepository.ListAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> ListAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _roleRepository.ListAsync(predicate, cancellationToken);
    }
}
