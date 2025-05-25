// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Features.Users.Interface;
using TraVinhMaps.Application.Features.Users.Models;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Domain.Specs;

namespace TraVinhMaps.Application.Features.Users;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRepository<Role> _roleRepository;

    public UserService(IUserRepository userRepository, IRepository<Role> roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<User> AddAdminAsync(AddAdminRequest request, CancellationToken cancellationToken = default)
    {
        return await _userRepository.AddAdminAsync(request, cancellationToken);
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

    public async Task<AdminProfileResponse> GetProfileAdmin(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("User not found!");
        }
        var role = await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
        var adminProfile = new AdminProfileResponse
        {
            Id = user.Id,
            UserName = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Password = user.Password, // Consider hashing this before returning
            RoleName = role?.RoleName ?? "admin",
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Avatar = user.Profile?.Avatar,
            IsForbidden = user.IsForbidden,
            Status = user.Status
        };
        return adminProfile;
    }
}
