// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.UnitOfWorks;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
}
