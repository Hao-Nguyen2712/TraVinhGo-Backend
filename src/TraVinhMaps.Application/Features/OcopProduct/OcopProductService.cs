// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.OcopProduct.Interface;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Specs;

namespace TraVinhMaps.Application.Features.OcopProduct;
public class OcopProductService : IOcopProductService
{
    private readonly IOcopProductRepository _ocopProductRepository;
    public OcopProductService(IOcopProductRepository ocopProductRepository)
    {
        _ocopProductRepository = ocopProductRepository;
    }
    public Task<Domain.Entities.OcopProduct> AddAsync(Domain.Entities.OcopProduct entity, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.AddAsync(entity, cancellationToken);
    }

    public Task<IEnumerable<Domain.Entities.OcopProduct>> AddRangeAsync(IEnumerable<Domain.Entities.OcopProduct> entities, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.AddRangeAsync(entities, cancellationToken);
    }

    public Task<long> CountAsync(Expression<Func<Domain.Entities.OcopProduct, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.CountAsync(predicate, cancellationToken);
    }

    //public Task DeleteAsync(Domain.Entities.OcopProduct entity, CancellationToken cancellationToken = default)
    //{
    //    return _ocopProductRepository.DeleteAsync(entity, cancellationToken);
    //}
    public Task<bool> DeleteOcopProductAsync(string id, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.DeleteOcopProductAsync(id, cancellationToken);
    }
    public Task<bool> RestoreOcopProductAsync(string id, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.RestoreOcopProductAsync(id, cancellationToken);
    }

    public Task<Pagination<Domain.Entities.OcopProduct>> GetAllOcopProductAsync(OcopProductSpecParams ocopProductSpecParams)
    {
        return _ocopProductRepository.GetAllOcopProductAsync(ocopProductSpecParams);
    }
    public Task<Domain.Entities.OcopProduct> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<IEnumerable<Domain.Entities.OcopProduct>> GetOcopProductByCompanyId(string companyId, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.GetOcopProductByCompanyId(companyId, cancellationToken);
    }

    public Task<IEnumerable<Domain.Entities.OcopProduct>> GetOcopProductByOcopTypeId(string ocopTypeId, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.GetOcopProductByOcopTypeId(ocopTypeId, cancellationToken);
    }

    public Task<IEnumerable<Domain.Entities.OcopProduct>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.ListAllAsync(cancellationToken);
    }

    public Task<IEnumerable<Domain.Entities.OcopProduct>> ListAsync(Expression<Func<Domain.Entities.OcopProduct, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.ListAsync(predicate, cancellationToken);
    }

    public Task UpdateAsync(Domain.Entities.OcopProduct entity, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.UpdateAsync(entity, cancellationToken);
    }
    public Task<bool> UpdateOcopProductAsync(Domain.Entities.OcopProduct entity, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.UpdateOcopProductAsync(entity, cancellationToken);
    }

    public Task<String> AddImageOcopProduct(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.AddImageOcopProduct(id, imageUrl, cancellationToken);
    }

    public Task<IEnumerable<Domain.Entities.OcopProduct>> GetAllOcopProductActiveAsync(CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.GetAllOcopProductActiveAsync(cancellationToken);
    }
}
