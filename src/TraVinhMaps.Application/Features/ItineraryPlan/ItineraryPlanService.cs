// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.ItineraryPlan.Interface;
using TraVinhMaps.Application.UnitOfWorks;

namespace TraVinhMaps.Application.Features.ItineraryPlan;
public class ItineraryPlanService : IItineraryPlanService
{
    private readonly IItineraryPlanRepository _repository;

    public ItineraryPlanService(IItineraryPlanRepository repository)
    {
        _repository = repository;
    }

    public async Task<Domain.Entities.ItineraryPlan> AddAsync(Domain.Entities.ItineraryPlan entity, CancellationToken cancellationToken = default)
    {
        return await _repository.AddAsync(entity, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.ItineraryPlan>> AddRangeAsync(IEnumerable<Domain.Entities.ItineraryPlan> entities, CancellationToken cancellationToken = default)
    {
        return await _repository.AddRangeAsync(entities, cancellationToken);
    }

    public async Task<long> CountAsync(Expression<Func<Domain.Entities.ItineraryPlan, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return await _repository.CountAsync(predicate, cancellationToken);
    }

    public Task DeleteAsync(Domain.Entities.ItineraryPlan entity, CancellationToken cancellationToken = default)
    {
        return _repository.DeleteAsync(entity, cancellationToken);
    }

    public async Task<Domain.Entities.ItineraryPlan> GetAsyns(Expression<Func<Domain.Entities.ItineraryPlan, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _repository.GetAsyns(predicate, cancellationToken);
    }

    public async Task<Domain.Entities.ItineraryPlan> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.ItineraryPlan>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.ListAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.ItineraryPlan>> ListAsync(Expression<Func<Domain.Entities.ItineraryPlan, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _repository.ListAsync(predicate, cancellationToken);
    }

    public Task UpdateAsync(Domain.Entities.ItineraryPlan entity, CancellationToken cancellationToken = default)
    {
        return _repository.UpdateAsync(entity, cancellationToken);
    }
}
