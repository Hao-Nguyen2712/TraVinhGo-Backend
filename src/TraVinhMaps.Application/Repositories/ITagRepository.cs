// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using TraVinhMaps.Application.UnitOfWorks;

namespace TraVinhMaps.Application.Repositories;
public interface ITagRepository : IBaseRepository<Domain.Entities.Tags>
{
    Task<string> GetTagIdByNameAsync(string tagName, CancellationToken cancellationToken = default);
}
