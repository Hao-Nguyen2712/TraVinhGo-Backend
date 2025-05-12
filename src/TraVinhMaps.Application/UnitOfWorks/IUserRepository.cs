// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Domain.Specs;

namespace TraVinhMaps.Application.UnitOfWorks;
public interface IUserRepository : IRepository<User>
{
    // Task<long> CountAsync(Expression<Func<User, bool>> predicate = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteUser(string id, CancellationToken cancellationToken = default);
    Task<bool> RestoreUser(string id, CancellationToken cancellationToken = default);
    Task<Pagination<User>> GetUserAsync(UserSpecParams  userSpecParams , CancellationToken cancellationToken = default);
    Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
}
