// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.Feedback.Models;

namespace TraVinhMaps.Application.Features.Feedback.Interface;
public interface IFeedbackService
{
    Task<FeedbackResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeedbackResponse>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Feedback>> ListAsync(Expression<Func<Domain.Entities.Feedback, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Feedback> AddAsync(FeedbackRequest entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Feedback>> AddRangeAsync(IEnumerable<Domain.Entities.Feedback> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(Domain.Entities.Feedback entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Domain.Entities.Feedback entity, CancellationToken cancellationToken = default);
}
