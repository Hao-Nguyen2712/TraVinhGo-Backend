// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.Admins.Models;
using TraVinhMaps.Application.Features.Users.Models;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Domain.Specs;

namespace TraVinhMaps.Application.Features.Admins.Interface;
public interface IAdminService
{
    Task<User> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> ListAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default);
    Task<User> AddAsync(AdminRequest entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> AddRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(UpdateAdminRequest entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(User entity, CancellationToken cancellationToken = default);
    Task<long> CountAsync(Expression<Func<User, bool>> predicate = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteAdmin(string id, CancellationToken cancellationToken = default);
    Task<bool> RestoreAdmin(string id, CancellationToken cancellationToken = default);
}
