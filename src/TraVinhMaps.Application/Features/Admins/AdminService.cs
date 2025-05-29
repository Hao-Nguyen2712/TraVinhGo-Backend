// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.External;
using TraVinhMaps.Application.Features.Admins.Interface;
using TraVinhMaps.Application.Features.Admins.Models;
using TraVinhMaps.Application.Features.Roles.Interface;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Admins;
public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IRoleService _roleService;
    private readonly IEmailSender _emailSender;

    public AdminService(IAdminRepository adminRepository, IEmailSender emailSender, IRoleService roleService = null)
    {
        _adminRepository = adminRepository;
        _emailSender = emailSender;
        _roleService = roleService;
    }

    public async Task<User> AddAsync(AdminRequest entity, CancellationToken cancellationToken = default)
    {
        // Get the admin role from the role service
        var adminRole = (await _roleService.ListAllAsync(cancellationToken))
            .FirstOrDefault(r => r.RoleName.ToLower() == "admin" && r.RoleStatus);
        if (adminRole == null)
        {
            throw new BadRequestException("Admin role not found.");
        }

        // Assign the admin role ID to the new entity
        entity.RoleId = adminRole.Id;

        // Create the admin user using the repository
        var admin = await _adminRepository.AddAsync(entity, cancellationToken);
        if (admin == null)
        {
            throw new BadRequestException("User not created succesfully");
        }

        //  Send email notification with the password
        await _emailSender.SendEmailAsync(
    admin.Email,
    "Welcome to TraVinhMaps - Admin Account Created",
    $"Hello {admin.Username ?? "Admin"},\n\n" +
    $"Your administrator account has been created.\n" +
    $"Email: {admin.Email}\n" +
    $"Temporary Password: admin123@\n\n" +
    $"\n\nPlease log in and change your password as soon as possible for security.\n\n" +
    $"Thank you,\nTraVinhMaps Team"
);
        return admin;
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
        var adminRole = (await _roleService.ListAllAsync())
        .FirstOrDefault(r => r.RoleName.ToLower() == "admin" && r.RoleStatus);

        if (adminRole == null)
        {
            throw new NotFoundException("Admin role not found.");
        }

        // Return all users with admin role
        return await _adminRepository.ListAsync(u => u.RoleId == adminRole.Id, cancellationToken);
    }

    public async Task<IEnumerable<User>> ListAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _adminRepository.ListAsync(predicate, cancellationToken);
    }

    public async Task<bool> RestoreAdmin(string id, CancellationToken cancellationToken = default)
    {
        return await _adminRepository.RestoreAdmin(id, cancellationToken);
    }

    //public async Task<User> UpdateAsync(UpdateAdminRequest entity, CancellationToken cancellationToken = default)
    //{
    //    return await _adminRepository.UpdateAsync(entity, cancellationToken);
    //}
}
