// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;

namespace TraVinhMaps.Application.Features.OcopProduct.Models;
public class CreateOcopProductRequest
{
    public required string ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public List<IFormFile>? ProductImageFile { get; set; }
    public string? ProductPrice { get; set; }
    public required string OcopTypeId { get; set; }
    public required string CompanyId { get; set; }
    public required int OcopPoint { get; set; }
    public required int OcopYearRelease { get; set; }
    public required string TagId { get; set; }
}

