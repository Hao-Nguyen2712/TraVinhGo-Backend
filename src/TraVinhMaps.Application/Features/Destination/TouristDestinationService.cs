// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using TraVinhMaps.Application.Features.Destination.Interface;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Domain.Specs;

namespace TraVinhMaps.Application.Features.Destination;
public class TouristDestinationService : ITouristDestinationService
{
    private readonly ITouristDestinationRepository _repository;
    public TouristDestinationService(ITouristDestinationRepository repository)
    {
        _repository = repository;
    }
    public async Task<TouristDestination> AddAsync(TouristDestination entity, CancellationToken cancellationToken = default)
    {
        return await _repository.AddAsync(entity, cancellationToken);
    }

    public async Task<string> AddDestinationHistoryStoryImage(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        return await _repository.AddDestinationHistoryStoryImage(id, imageUrl, cancellationToken);
    }

    public async Task<string> AddDestinationImage(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        return await _repository.AddDestinationImage(id, imageUrl, cancellationToken);
    }

    public async Task<IEnumerable<TouristDestination>> AddRangeAsync(IEnumerable<TouristDestination> entities, CancellationToken cancellationToken = default)
    {
        return await _repository.AddRangeAsync(entities, cancellationToken);
    }

    public async Task<long> CountAsync(Expression<Func<TouristDestination, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return await _repository.CountAsync(predicate, cancellationToken);
    }

    public Task DeleteAsync(TouristDestination entity, CancellationToken cancellationToken = default)
    {
        return _repository.DeleteAsync(entity, cancellationToken);
    }

    public async Task<string> DeleteDestinationHistoryStoryImage(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        return await _repository.DeleteDestinationHistoryStoryImage(id, imageUrl, cancellationToken);
    }

    public async Task<string> DeleteDestinationImage(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        return await _repository.DeleteDestinationImage(id, imageUrl, cancellationToken);
    }

    public async Task<TouristDestination> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<TouristDestination>> GetByTagIdAsync(string tagId, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByTagIdAsync(tagId, cancellationToken);
    }

    public async Task<Pagination<TouristDestination>> GetTouristDestination(TouristDestinationSpecParams touristDestinationSpecParams, CancellationToken cancellationToken = default)
    {
        return await _repository.GetTouristDestination(touristDestinationSpecParams, cancellationToken);
    }

    public async Task<IEnumerable<TouristDestination>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.ListAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<TouristDestination>> ListAsync(Expression<Func<TouristDestination, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _repository.ListAsync(predicate, cancellationToken);
    }

    public async Task<bool> PlusFavorite(string id, CancellationToken cancellationToken = default)
    {
        return await _repository.PlusFavorite(id, cancellationToken);
    }

    public Task UpdateAsync(TouristDestination entity, CancellationToken cancellationToken = default)
    {
        return _repository.UpdateAsync(entity, cancellationToken);
    }
}
