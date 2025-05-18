// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Domain.Specs;

namespace TraVinhMaps.Application.Features.CommunityTips.Interface;
public interface ICommunityTipsService
{
    Task<Domain.Entities.Tips> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Tips>> ListAllAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Lists the asynchronous with the conditional
    /// </summary>
    /// <param name="predicate">The predicate is the conditional when you list (ex : a => a.id == 1)</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<IEnumerable<Domain.Entities.Tips>> ListAsync(Expression<Func<Domain.Entities.Tips, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Tips>> GetAllTipActiveAsync(CancellationToken cancellationToken = default);
    Task<Domain.Entities.Tips> AddAsync(Domain.Entities.Tips entity, CancellationToken cancellationToken = default);
    /// <summary>
    /// Adds the range asynchronous with conditional
    /// </summary>
    /// <param name="entities">The entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<IEnumerable<Domain.Entities.Tips>> AddRangeAsync(IEnumerable<Domain.Entities.Tips> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(Domain.Entities.Tips entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Domain.Entities.Tips entity, CancellationToken cancellationToken = default);
    Task DeleteTipAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> RestoreTipAsync(string id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Counts the asynchronous.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<long> CountAsync(Expression<Func<Domain.Entities.Tips, bool>> predicate = null, CancellationToken cancellationToken = default);
}
