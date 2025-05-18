// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Domain.Specs;

namespace TraVinhMaps.Application.Features.OcopProduct.Interface;
public interface IOcopProductService
{
    Task<Domain.Entities.OcopProduct> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.OcopProduct>> ListAllAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Lists the asynchronous with the conditional
    /// </summary>
    /// <param name="predicate">The predicate is the conditional when you list (ex : a => a.id == 1)</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<IEnumerable<Domain.Entities.OcopProduct>> ListAsync(Expression<Func<Domain.Entities.OcopProduct, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.OcopProduct>> GetAllOcopProductActiveAsync(CancellationToken cancellationToken = default);
    Task<Domain.Entities.OcopProduct> AddAsync(Domain.Entities.OcopProduct entity, CancellationToken cancellationToken = default);
    /// <summary>
    /// Adds the range asynchronous with conditional
    /// </summary>
    /// <param name="entities">The entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<IEnumerable<Domain.Entities.OcopProduct>> AddRangeAsync(IEnumerable<Domain.Entities.OcopProduct> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(Domain.Entities.OcopProduct entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Domain.Entities.OcopProduct entity, CancellationToken cancellationToken = default);
    Task DeleteOcopProductAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> RestoreOcopProductAsync(string id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Counts the asynchronous.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<long> CountAsync(Expression<Func<Domain.Entities.OcopProduct, bool>> predicate = null, CancellationToken cancellationToken = default);
    Task<Pagination<Domain.Entities.OcopProduct>> GetAllOcopProductAsync(OcopProductSpecParams ocopProductSpecParams);
    Task<IEnumerable<Domain.Entities.OcopProduct>> GetOcopProductByOcopTypeId(string ocopTypeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.OcopProduct>> GetOcopProductByCompanyId(string companyId, CancellationToken cancellationToken = default);
    Task<String> AddImageOcopProduct(string id, string imageUrl, CancellationToken cancellationToken = default);

}
