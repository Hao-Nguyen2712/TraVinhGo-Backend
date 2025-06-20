// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.Tags.Interface;
using TraVinhMaps.Application.Repositories;

namespace TraVinhMaps.Application.Features.Tags;
public class TagService : ITagService
{
    private readonly ITagRepository _tagsRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagsRepository = tagRepository;
    }

    public async Task<Domain.Entities.Tags> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _tagsRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<string> GetTagIdByNameAsync(string tagName, CancellationToken cancellationToken = default)
    {
        return await _tagsRepository.GetTagIdByNameAsync(tagName, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Tags>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _tagsRepository.ListAllAsync(cancellationToken);
    }
}
