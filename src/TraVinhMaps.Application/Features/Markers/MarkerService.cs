// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.Markers.Interface;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Markers;
public class MarkerService : IMarkerService
{
    private readonly IMarkerRepository _repository;
    public MarkerService(IMarkerRepository repository)
    {
        _repository = repository;
    }
    public async Task<Marker> AddAsync(Marker entity, CancellationToken cancellationToken = default)
    {
        return await _repository.AddAsync(entity, cancellationToken);
    }

    public async Task<string> AddMarkerImage(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        return await _repository.AddMarkerImage(id, imageUrl, cancellationToken);
    }

    public async Task<IEnumerable<Marker>> AddRangeAsync(IEnumerable<Marker> entities, CancellationToken cancellationToken = default)
    {
        return await _repository.AddRangeAsync(entities, cancellationToken);
    }

    public async Task<long> CountAsync(Expression<Func<Marker, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return await _repository.CountAsync(predicate, cancellationToken);
    }

    public Task DeleteAsync(Marker entity, CancellationToken cancellationToken = default)
    {
        return _repository.DeleteAsync(entity, cancellationToken);
    }

    public async Task<Marker> GetAsyns(Expression<Func<Marker, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _repository.GetAsyns(predicate, cancellationToken);
    }

    public async Task<Marker> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Marker>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.ListAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<Marker>> ListAsync(Expression<Func<Marker, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _repository.ListAsync(predicate, cancellationToken);
    }

    public Task UpdateAsync(Marker entity, CancellationToken cancellationToken = default)
    {
        return _repository.UpdateAsync(entity, cancellationToken);    
    }
}
