// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.ItineraryPlan.Interface;
public interface IItineraryPlanService
{
    Task<TraVinhMaps.Domain.Entities.ItineraryPlan> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TraVinhMaps.Domain.Entities.ItineraryPlan>> ListAllAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Lists the asynchronous with the conditional
    /// </summary>
    /// <param name="predicate">The predicate is the conditional when you list (ex : a => a.id == 1)</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<IEnumerable<TraVinhMaps.Domain.Entities.ItineraryPlan>> ListAsync(Expression<Func<TraVinhMaps.Domain.Entities.ItineraryPlan, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TraVinhMaps.Domain.Entities.ItineraryPlan> AddAsync(TraVinhMaps.Domain.Entities.ItineraryPlan entity, CancellationToken cancellationToken = default);
    /// <summary>
    /// Adds the range asynchronous with conditional
    /// </summary>
    /// <param name="entities">The entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<IEnumerable<TraVinhMaps.Domain.Entities.ItineraryPlan>> AddRangeAsync(IEnumerable<TraVinhMaps.Domain.Entities.ItineraryPlan> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(TraVinhMaps.Domain.Entities.ItineraryPlan entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TraVinhMaps.Domain.Entities.ItineraryPlan entity, CancellationToken cancellationToken = default);
    /// <summary>
    /// Counts the asynchronous.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<long> CountAsync(Expression<Func<TraVinhMaps.Domain.Entities.ItineraryPlan, bool>> predicate = null, CancellationToken cancellationToken = default);
    Task<TraVinhMaps.Domain.Entities.ItineraryPlan> GetAsyns(Expression<Func<TraVinhMaps.Domain.Entities.ItineraryPlan, bool>> predicate, CancellationToken cancellationToken = default);
}
