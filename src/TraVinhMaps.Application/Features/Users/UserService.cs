// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using TraVinhMaps.Application.Features.Users.Interface;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Domain.Specs;

namespace TraVinhMaps.Application.Features.Users;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        return await _userRepository.AddAsync(entity, cancellationToken);
    }

    public async Task<IEnumerable<User>> AddRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken = default)
    {
        return await _userRepository.AddRangeAsync(entities, cancellationToken);
    }

    public async Task<long> CountAsync(Expression<Func<User, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return await _userRepository.CountAsync(predicate, cancellationToken);
    }

    public async Task DeleteAsync(User entity, CancellationToken cancellationToken = default)
    {
        await _userRepository.DeleteAsync(entity, cancellationToken);
    }

    public async Task<bool> DeleteUser(string id, CancellationToken cancellationToken = default)
    {
        return await _userRepository.DeleteUser(id, cancellationToken);
    }

    public async Task<User> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Pagination<User>> GetUsersAsync(UserSpecParams userSpecParams, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetUserAsync(userSpecParams, cancellationToken);
    }

    public async Task<IEnumerable<User>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _userRepository.ListAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> ListAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _userRepository.ListAsync(predicate, cancellationToken);
    }

    public async Task<bool> RestoreUser(string id, CancellationToken cancellationToken = default)
    {
        return await _userRepository.RestoreUser(id, cancellationToken);
    }

    public async Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        await _userRepository.UpdateAsync(entity, cancellationToken);
    }
}
