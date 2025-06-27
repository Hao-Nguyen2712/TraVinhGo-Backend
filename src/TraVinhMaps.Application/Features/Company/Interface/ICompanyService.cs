// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TraVinhMaps.Application.Features.Company.Interface;
public interface ICompanyService
{
    Task<Domain.Entities.Company> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Company>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<Domain.Entities.Company> AddAsync(Domain.Entities.Company entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Domain.Entities.Company entity, CancellationToken cancellationToken = default);
    Task<long> CountAsync(Expression<Func<Domain.Entities.Company, bool>> predicate = null, CancellationToken cancellationToken = default);
}
