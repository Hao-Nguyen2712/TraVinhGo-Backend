// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization.Attributes;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.EventAndFestivalFeature.Models;
public class CreateEventAndFestivalRequest
{
    public required string NameEvent { get; set; }
    public string? Description { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public string? Category { get; set; }
    public required List<IFormFile> ImagesFile { get; set; }
    public required EventLocation Location { get; set; }
    public required string TagId { get; set; }
}
