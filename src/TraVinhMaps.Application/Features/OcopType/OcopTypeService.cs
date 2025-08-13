// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using TraVinhMaps.Application.Features.OcopType.Interface;
using TraVinhMaps.Application.Repositories;
using TraVinhMaps.Application.UnitOfWorks;

namespace TraVinhMaps.Application.Features.OcopType;

public class OcopTypeService : IOcopTypeService
{
    private readonly IBaseRepository<Domain.Entities.OcopType> _repository;
    private readonly IOcopTypeRepository _ocopTypeRepository;
    private readonly IBaseRepository<Domain.Entities.OcopProduct> _ocopProductRepository;
    public OcopTypeService(IBaseRepository<Domain.Entities.OcopType> repository, IOcopTypeRepository ocopTypeRepository, IBaseRepository<Domain.Entities.OcopProduct> ocopProductRepository)
    {
        _repository = repository;
        _ocopTypeRepository = ocopTypeRepository;
        _ocopProductRepository = ocopProductRepository;
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

    public Task<Domain.Entities.OcopType> GetOcopTypeByName(string name, CancellationToken cancellationToken = default)
    {
        return _ocopTypeRepository.GetOcopTypeByName(name, cancellationToken);
    }

    public Task<IEnumerable<Domain.Entities.OcopType>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return _repository.ListAllAsync(cancellationToken);
    }
    public Task UpdateAsync(Domain.Entities.OcopType entity, CancellationToken cancellationToken = default)
    {
        return _repository.UpdateAsync(entity, cancellationToken);
    }

    public async Task<int> CountOcopProductsByTypeIdAsync(string typeId, CancellationToken cancellationToken = default)
    {
        var result = await _ocopProductRepository.CountAsync(x => x.OcopTypeId == typeId, cancellationToken);
        if ((int)result > 0)
        {
            return (int)result;
        }
        var ocopType = await _repository.GetByIdAsync(typeId, cancellationToken);
        await _ocopTypeRepository.DeleteAsync(ocopType, cancellationToken);
        return 0;
    }
}
