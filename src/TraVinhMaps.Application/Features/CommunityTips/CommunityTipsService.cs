// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.CommunityTips.Interface;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.CommunityTips;
public class CommunityTipsService : ICommunityTipsService
{
    private readonly ICommunityTipsRepository _communityTipsRepository;
    public CommunityTipsService(ICommunityTipsRepository communityTipsRepository)
    {
        _communityTipsRepository = communityTipsRepository;
    }
    public Task<Tips> AddAsync(Domain.Entities.Tips entity, CancellationToken cancellationToken = default)
    {
        return _communityTipsRepository.AddAsync(entity, cancellationToken);
    }

    public Task<IEnumerable<Tips>> AddRangeAsync(IEnumerable<Domain.Entities.Tips> entities, CancellationToken cancellationToken = default)
    {
        return _communityTipsRepository.AddRangeAsync(entities, cancellationToken);
    }

    public Task<long> CountAsync(Expression<Func<Tips, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return _communityTipsRepository.CountAsync(predicate, cancellationToken);
    }

    public Task DeleteAsync(Domain.Entities.Tips entity, CancellationToken cancellationToken = default)
    {
        return _communityTipsRepository.DeleteAsync(entity, cancellationToken);
    }

    public Task DeleteTipAsync(string id, CancellationToken cancellationToken = default)
    {
        return _communityTipsRepository.DeleteTipAsync(id, cancellationToken);
    }

    public Task<IEnumerable<Tips>> GetAllTipActiveAsync(CancellationToken cancellationToken = default)
    {
        return _communityTipsRepository.GetAllTipActiveAsync(cancellationToken);
    }

    public Task<Tips> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _communityTipsRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<IEnumerable<Tips>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return _communityTipsRepository.ListAllAsync(cancellationToken);
    }

    public Task<IEnumerable<Tips>> ListAsync(Expression<Func<Domain.Entities.Tips, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _communityTipsRepository.ListAsync(predicate, cancellationToken);
    }

    public Task<bool> RestoreTipAsync(string id, CancellationToken cancellationToken = default)
    {
        return _communityTipsRepository.RestoreTipAsync(id, cancellationToken);
    }

    public Task UpdateAsync(Tips entity, CancellationToken cancellationToken = default)
    {
        return _communityTipsRepository.UpdateAsync(entity, cancellationToken);
    }
}
