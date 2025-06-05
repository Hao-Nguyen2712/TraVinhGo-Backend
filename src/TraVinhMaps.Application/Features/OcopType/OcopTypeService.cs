// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.OcopType.Interface;
using TraVinhMaps.Application.UnitOfWorks;

namespace TraVinhMaps.Application.Features.OcopType;
public class OcopTypeService : IOcopTypeService
{
    private readonly IRepository<Domain.Entities.OcopType> _repository;
    public OcopTypeService(IRepository<Domain.Entities.OcopType> repository)
    {
        _repository = repository;
    }
    public Task<Domain.Entities.OcopType> AddAsync(Domain.Entities.OcopType entity, CancellationToken cancellationToken = default)
    {
        return _repository.AddAsync(entity, cancellationToken);
    }
    public Task<long> CountAsync(Expression<Func<Domain.Entities.OcopType, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return _repository.CountAsync(predicate, cancellationToken);
    }

    public Task<Domain.Entities.OcopType> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(id, cancellationToken);
    }

    public Task<IEnumerable<Domain.Entities.OcopType>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return _repository.ListAllAsync(cancellationToken);
    }
    public Task UpdateAsync(Domain.Entities.OcopType entity, CancellationToken cancellationToken = default)
    {
        return _repository.UpdateAsync(entity, cancellationToken);
    }
}
