// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Markers.Interface;
public interface IMarkerService
{
    Task<Marker> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Marker>> ListAllAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Lists the asynchronous with the conditional
    /// </summary>
    /// <param name="predicate">The predicate is the conditional when you list (ex : a => a.id == 1)</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<IEnumerable<Marker>> ListAsync(Expression<Func<Marker, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Marker> AddAsync(Marker entity, CancellationToken cancellationToken = default);
    /// <summary>
    /// Adds the range asynchronous with conditional
    /// </summary>
    /// <param name="entities">The entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<IEnumerable<Marker>> AddRangeAsync(IEnumerable<Marker> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(Marker entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Marker entity, CancellationToken cancellationToken = default);
    /// <summary>
    /// Counts the asynchronous.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<long> CountAsync(Expression<Func<Marker, bool>> predicate = null, CancellationToken cancellationToken = default);
    Task<Marker> GetAsyns(Expression<Func<Marker, bool>> predicate, CancellationToken cancellationToken = default);
    Task<string> AddMarkerImage(string id, string imageUrl, CancellationToken cancellationToken = default);
}
