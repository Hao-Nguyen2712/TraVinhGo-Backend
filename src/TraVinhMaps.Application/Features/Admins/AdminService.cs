// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using TraVinhMaps.Application.Features.Admins.Interface;
using TraVinhMaps.Application.Features.Admins.Models;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Admins;
public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;

    public AdminService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<User> AddAsync(AdminRequest entity, CancellationToken cancellationToken = default)
    {
        return await _adminRepository.AddAsync(entity, cancellationToken);
    }

    public async Task<IEnumerable<User>> AddRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken = default)
    {
        return await _adminRepository.AddRangeAsync(entities, cancellationToken);
    }

    public async Task<long> CountAsync(Expression<Func<User, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return await _adminRepository.CountAsync(predicate, cancellationToken);
    }

    public async Task DeleteAsync(User entity, CancellationToken cancellationToken = default)
    {
        await _adminRepository.DeleteAsync(entity, cancellationToken);
    }

    public async Task<bool> DeleteAdmin(string id, CancellationToken cancellationToken = default)
    {
        return await _adminRepository.DeleteAdmin(id, cancellationToken);
    }

    public async Task<User> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
       return await _adminRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<User>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _adminRepository.ListAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> ListAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _adminRepository.ListAsync(predicate, cancellationToken);
    }

    public async Task<bool> RestoreAdmin(string id, CancellationToken cancellationToken = default)
    {
        return await _adminRepository.RestoreAdmin(id, cancellationToken);
    }

    public async Task<User> UpdateAsync(UpdateAdminRequest entity, CancellationToken cancellationToken = default)
    {
        return await _adminRepository.UpdateAsync(entity, cancellationToken);
    }
}
