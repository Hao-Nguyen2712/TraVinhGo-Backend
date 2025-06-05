// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.UnitOfWorks;
public interface IEventAndFestivalRepository : IBaseRepository<EventAndFestival>
{
    Task<string> AddEventAndFestivalImage(string id, string imageUrl, CancellationToken cancellationToken = default);
    Task<string> DeleteEventAndFestivalImage(string id, string imageUrl, CancellationToken cancellationToken = default);
}
