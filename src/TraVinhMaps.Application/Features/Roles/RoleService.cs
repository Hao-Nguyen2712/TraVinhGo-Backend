// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TraVinhMaps.Application.Features.Roles.Interface;
using TraVinhMaps.Application.Features.Roles.Models;
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

    public async Task<Role> AddAsync(RoleRequest entity, CancellationToken cancellationToken = default)
    {
        return await _roleRepository.AddAsync(entity, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
         return await _roleRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<Role> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _roleRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Role>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _roleRepository.ListAllAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(string id, RoleRequest entity, CancellationToken cancellationToken = default)
    {
        return await _roleRepository.UpdateAsync(id, entity, cancellationToken);
    }
}
