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
public interface ITouristDestinationRepository : IRepository<TouristDestination>
{
    Task<IEnumerable<TouristDestination>> GetByTagIdAsync(string tagId, CancellationToken cancellationToken = default);
    Task<String> AddDestinationImage(string id ,string imageUrl, CancellationToken cancellationToken = default);
    Task<String> DeleteDestinationImage( string id,string imageUrl, CancellationToken cancellationToken = default);
    Task<String> AddDestinationHistoryStoryImage(string id, string imageUrl, CancellationToken cancellationToken= default);
    Task<String> DeleteDestinationHistoryStoryImage(string id, string imageUrl, CancellationToken cancellationToken= default);
    Task<Pagination<TouristDestination>> GetTouristDestination(TouristDestinationSpecParams touristDestinationSpecParams, CancellationToken cancellationToken = default);
    Task<bool> PlusFavorite(string id,CancellationToken cancellationToken = default);
}
