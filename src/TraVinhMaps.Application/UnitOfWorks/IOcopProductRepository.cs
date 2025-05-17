// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Domain.Specs;

namespace TraVinhMaps.Application.UnitOfWorks;
public interface IOcopProductRepository : IRepository<OcopProduct>
{
    Task<Pagination<OcopProduct>> GetAllOcopProductAsync(OcopProductSpecParams ocopProductSpecParams);
    Task<IEnumerable<OcopProduct>> GetAllOcopProductActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<OcopProduct>> GetOcopProductByOcopTypeId(string ocopTypeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OcopProduct>> GetOcopProductByCompanyId(string companyId, CancellationToken cancellationToken = default);
    Task<bool> DeleteOcopProductAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> RestoreOcopProductAsync(string id, CancellationToken cancellationToken = default);
    Task<String> AddImageOcopProduct(string id, string imageUrl, CancellationToken cancellationToken = default);
}
