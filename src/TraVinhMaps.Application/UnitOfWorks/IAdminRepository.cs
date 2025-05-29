// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.Admins.Models;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.UnitOfWorks;
public interface IAdminRepository : IRepository<User>
{
    Task<User> AddAsync(AdminRequest entity, CancellationToken cancellationToken);
    //Task<User> UpdateAsync(UpdateAdminRequest entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAdmin(string id, CancellationToken cancellationToken = default);
    Task<bool> RestoreAdmin(string id, CancellationToken cancellationToken = default);
}
