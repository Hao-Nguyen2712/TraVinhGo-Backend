// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.DestinationTypes.Interface;
public interface IDestinationTypeService
{
    Task<DestinationType> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DestinationType>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<DestinationType>> ListAsync(Expression<Func<DestinationType, bool>> predicate, CancellationToken cancellationToken = default);
    Task<DestinationType> AddAsync(DestinationType entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<DestinationType>> AddRangeAsync(IEnumerable<DestinationType> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(DestinationType entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(DestinationType entity, CancellationToken cancellationToken = default);
    Task<long> CountAsync(Expression<Func<DestinationType, bool>> predicate = null, CancellationToken cancellationToken = default);
    Task<DestinationType> GetAsyns(Expression<Func<DestinationType, bool>> predicate, CancellationToken cancellationToken = default);
}
