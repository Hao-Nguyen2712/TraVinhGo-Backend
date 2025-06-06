// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Application.Features.OcopProduct.Models;
public class ProductLookUpsResponse
{
    public List<OcopTypeDto> OcopTypes { get; set; } = new List<OcopTypeDto>();
    public List<CompanyDto> Companies { get; set; } = new List<CompanyDto>();
    public TagDto Tags { get; set; } = new TagDto(); // Assuming a single tag for simplicity
}

public class OcopTypeDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class CompanyDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class TagDto //
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
