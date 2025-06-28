// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TraVinhMaps.Application.Features.InteractionLogs.Interface;
using TraVinhMaps.Application.Features.InteractionLogs.Mappers;
using TraVinhMaps.Application.Features.InteractionLogs.Models;
using TraVinhMaps.Application.Features.Users.Interface;
using TraVinhMaps.Application.UnitOfWorks;

namespace TraVinhMaps.Application.Features.InteractionLogs;
public class InteractionLogsService : IInteractionLogsService
{
    private readonly IBaseRepository<Domain.Entities.InteractionLogs> _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    public InteractionLogsService(IBaseRepository<Domain.Entities.InteractionLogs> repository, IHttpContextAccessor httpContextAccessor, IUserService userService)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public Task<Domain.Entities.InteractionLogs> AddAsync(CreateInteractionLogsRequest entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Lấy userId từ ClaimsPrincipal (được gán bởi SessionAuthenticationHandler) qua IHttpContextAccessor
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User not authenticated.");

        var createInteractionLogs = InteractionLogsMapper.Mapper.Map<Domain.Entities.InteractionLogs>(entity);
        createInteractionLogs.UserId = userId;

        return _repository.AddAsync(createInteractionLogs, cancellationToken);
    }

    public Task<long> CountAsync(Expression<Func<Domain.Entities.InteractionLogs, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return _repository.CountAsync(predicate, cancellationToken);
    }

    public Task DeleteAsync(Domain.Entities.InteractionLogs entity, CancellationToken cancellationToken = default)
    {
        return _repository.DeleteAsync(entity, cancellationToken);
    }

    public Task<Domain.Entities.InteractionLogs> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(id, cancellationToken);
    }

    public Task<IEnumerable<Domain.Entities.InteractionLogs>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return _repository.ListAllAsync(cancellationToken);
    }

    public Task UpdateAsync(Domain.Entities.InteractionLogs entity, CancellationToken cancellationToken = default)
    {
        return _repository.UpdateAsync(entity, cancellationToken);
    }
}
