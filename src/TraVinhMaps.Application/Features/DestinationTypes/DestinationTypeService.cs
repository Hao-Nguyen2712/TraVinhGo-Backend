// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.DestinationTypes.Interface;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.DestinationTypes;
public class DestinationTypeService : IDestinationTypeService
{
    private readonly IRepository<DestinationType> _repository;
    public DestinationTypeService(IRepository<DestinationType> repository)
    {
        _repository = repository;
    }
    public async Task<DestinationType> AddAsync(DestinationType entity, CancellationToken cancellationToken = default)
    {
        return await _repository.AddAsync(entity, cancellationToken);
    }

    public async Task<IEnumerable<DestinationType>> AddRangeAsync(IEnumerable<DestinationType> entities, CancellationToken cancellationToken = default)
    {
        return await _repository.AddRangeAsync(entities, cancellationToken);
    }

    public async Task<long> CountAsync(Expression<Func<DestinationType, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return await _repository.CountAsync(predicate, cancellationToken);
    }

    public  Task DeleteAsync(DestinationType entity, CancellationToken cancellationToken = default)
    {
        return _repository.DeleteAsync(entity, cancellationToken);
    }

    public async Task<DestinationType> GetAsyns(Expression<Func<DestinationType, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _repository.GetAsyns(predicate, cancellationToken);
    }

    public async Task<DestinationType> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);   
    }

    public async Task<IEnumerable<DestinationType>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.ListAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<DestinationType>> ListAsync(Expression<Func<DestinationType, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _repository.ListAsync(predicate, cancellationToken);
    }

    public Task UpdateAsync(DestinationType entity, CancellationToken cancellationToken = default)
    {
        return _repository.UpdateAsync(entity, cancellationToken);
    }
}
