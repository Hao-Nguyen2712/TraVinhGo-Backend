// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Destination.Models;
public class UpdateDestinationRequest
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public double? AvarageRating { get; set; }
    public string? Description { get; set; }
    public required string Address { get; set; }
    public required Location Location { get; set; }
    public HistoryStoryUpdateRequest? HistoryStory { get; set; }
    public required string DestinationTypeId { get; set; }
    public OpeningHours? OpeningHours { get; set; }
    public string? Capacity { get; set; }
    public Contact? Contact { get; set; }
    public required string TagId { get; set; }
    public string? Ticket { get; set; }
}
