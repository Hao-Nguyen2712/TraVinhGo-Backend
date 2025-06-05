// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TraVinhMaps.Application.Features.Admins.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.UnitOfWorks;
public interface IAdminRepository : IBaseRepository<User>
{
    Task<User> AddAsync(AdminRequest entity, CancellationToken cancellationToken);
    //Task<User> UpdateAsync(UpdateAdminRequest entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAdmin(string id, CancellationToken cancellationToken = default);
    Task<bool> RestoreAdmin(string id, CancellationToken cancellationToken = default);
}
