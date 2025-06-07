// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;

namespace TraVinhMaps.Application.Features.SellingLink.Interface;
public interface ISellingLinkService
{
    Task<Domain.Entities.SellingLink> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.SellingLink>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<Domain.Entities.SellingLink> AddAsync(Domain.Entities.SellingLink entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Domain.Entities.SellingLink entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Domain.Entities.SellingLink entity, CancellationToken cancellationToken = default);
    Task<long> CountAsync(Expression<Func<Domain.Entities.SellingLink, bool>> predicate = null, CancellationToken cancellationToken = default);
}
