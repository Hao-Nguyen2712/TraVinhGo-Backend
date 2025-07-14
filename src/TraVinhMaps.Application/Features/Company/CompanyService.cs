// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.Company.Interface;
using TraVinhMaps.Application.Repositories;
using TraVinhMaps.Application.UnitOfWorks;

namespace TraVinhMaps.Application.Features.Company;
public class CompanyService : ICompanyService
{
    private readonly IBaseRepository<Domain.Entities.Company> _repository;
    private readonly ICompanyRepository _companyRepository;
    public CompanyService(IBaseRepository<Domain.Entities.Company> repository, ICompanyRepository companyRepository)
    {
        _repository = repository;
        _companyRepository = companyRepository;
    }

    public Task<Domain.Entities.Company> AddAsync(Domain.Entities.Company entity, CancellationToken cancellationToken = default)
    {
        return _repository.AddAsync(entity, cancellationToken);
    }

    public Task<long> CountAsync(Expression<Func<Domain.Entities.Company, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return _repository.CountAsync(predicate, cancellationToken);
    }

    public Task<Domain.Entities.Company> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(id, cancellationToken);
    }

    public Task<Domain.Entities.Company> GetCompanyByName(string name, CancellationToken cancellationToken = default)
    {
        return _companyRepository.GetCompanyByName(name, cancellationToken);
    }

    public Task<IEnumerable<Domain.Entities.Company>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return _repository.ListAllAsync(cancellationToken);
    }

    public Task UpdateAsync(Domain.Entities.Company entity, CancellationToken cancellationToken = default)
    {
        return _repository.UpdateAsync(entity, cancellationToken);
    }
}
