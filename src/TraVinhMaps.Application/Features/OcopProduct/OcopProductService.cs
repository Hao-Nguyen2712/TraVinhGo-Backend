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
using TraVinhMaps.Domain.Entities;
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

    public Task<long> CountAsync(Expression<Func<Domain.Entities.OcopProduct, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.CountAsync(predicate, cancellationToken);
    }
    public Task<bool> DeleteOcopProductAsync(string id, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.DeleteOcopProductAsync(id, cancellationToken);
    }
    public Task<bool> RestoreOcopProductAsync(string id, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.RestoreOcopProductAsync(id, cancellationToken);
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

    public Task UpdateAsync(Domain.Entities.OcopProduct entity, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.UpdateAsync(entity, cancellationToken);
    }

    public Task<String> AddImageOcopProduct(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.AddImageOcopProduct(id, imageUrl, cancellationToken);
    }

    public Task<SellLocation> AddSellLocation(string id, SellLocation sellLocation, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.AddSellLocation(id, sellLocation, cancellationToken);
    }

    public Task<bool> DeleteSellLocation(string ocopProductId, string sellLocationName, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.DeleteSellLocation(ocopProductId, sellLocationName, cancellationToken);
    }

    public Task<bool> UpdateSellLocation(string id, SellLocation sellLocation, CancellationToken cancellationToken = default)
    {
        return _ocopProductRepository.UpdateSellLocation(id, sellLocation, cancellationToken);
    }
}
