// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.UnitOfWorks;
public interface ICommunityTipsRepository : IRepository<Tips>
{
    Task<bool> DeleteTipAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> RestoreTipAsync(string id, CancellationToken cancellationToken = default);
}
