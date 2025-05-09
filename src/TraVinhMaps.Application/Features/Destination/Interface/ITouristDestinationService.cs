// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Destination.Interface;
public interface ITouristDestinationService
{
    Task<TouristDestination> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TouristDestination>> ListAllAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Lists the asynchronous with the conditional
    /// </summary>
    /// <param name="predicate">The predicate is the conditional when you list (ex : a => a.id == 1)</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<IEnumerable<TouristDestination>> ListAsync(Expression<Func<TouristDestination, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TouristDestination> AddAsync(TouristDestination entity, CancellationToken cancellationToken = default);
    /// <summary>
    /// Adds the range asynchronous with conditional
    /// </summary>
    /// <param name="entities">The entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<IEnumerable<TouristDestination>> AddRangeAsync(IEnumerable<TouristDestination> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(TouristDestination entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TouristDestination entity, CancellationToken cancellationToken = default);
    /// <summary>
    /// Counts the asynchronous.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<long> CountAsync(Expression<Func<TouristDestination, bool>> predicate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<TouristDestination>> GetByTagIdAsync(string tagId, CancellationToken cancellationToken = default);
    Task<String> AddDestinationImage(string id, string imageUrl, CancellationToken cancellationToken = default);
    Task<String> DeleteDestinationImage(string id, string imageUrl, CancellationToken cancellationToken = default);
}
