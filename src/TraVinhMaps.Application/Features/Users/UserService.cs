// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.Common.Extensions;
using TraVinhMaps.Application.External;
using TraVinhMaps.Application.Features.Users.Interface;
using TraVinhMaps.Application.Features.Users.Models;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Domain.Specs;

namespace TraVinhMaps.Application.Features.Users;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public UserService(IUserRepository userRepository, IBaseRepository<Role> roleRepository, ICloudinaryService cloudinaryService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<User> AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        entity.Password = HashingTokenExtension.HashToken(entity.Password);
        entity.IsForbidden = false;
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

    public async Task<IEnumerable<User>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        var userRole = (await _roleRepository.ListAllAsync(cancellationToken))
             .FirstOrDefault(r => r.RoleName.ToLower() == "user" && r.RoleStatus);
        if (userRole == null)
        {
            throw new NotFoundException("User role not found.");
        }
        return await _userRepository.ListAsync(u => u.RoleId == userRole.Id, cancellationToken);
    }

    public async Task<IEnumerable<User>> ListAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _userRepository.ListAsync(u => u.Status == true, cancellationToken);
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

    public async Task UpdateProfileAdmin(UpdateProfileAdminRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return;
        }
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("User not found!");
        }
        if (request.Email != null)
        {
            var existingUser = await _userRepository.GetAsyns(u => u.Email == request.Email && u.Id != request.Id, cancellationToken);
            if (existingUser != null)
            {
                throw new BadRequestException("Email already exists!");
            }
            user.Email = request.Email;
        }
        if (request.PhoneNumber != null)
        {
            var existingUser = await _userRepository.GetAsyns(u => u.PhoneNumber == request.PhoneNumber && u.Id != request.Id, cancellationToken);
            if (existingUser != null)
            {
                throw new BadRequestException("Phone number already exists!");
            }
            user.PhoneNumber = request.PhoneNumber;
        }
        if (request.UserName != null)
        {
            user.Username = request.UserName;
        }
        if (request.Avartar != null)
        {
            var result = await _cloudinaryService.UploadImageAsync(request.Avartar);
            if (result == null || result.SecureUrl == null)
            {
                throw new BadRequestException("Failed to upload image!");
            }
            if (user.Profile == null)
            {
                user.Profile = new Profile(); // tạo mới nếu chưa có
            }
            user.Profile.Avatar = result.SecureUrl.ToString();
        }
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);
    }

    public Task<Pagination<User>> GetUsersAsync(UserSpecParams userSpecParams, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
