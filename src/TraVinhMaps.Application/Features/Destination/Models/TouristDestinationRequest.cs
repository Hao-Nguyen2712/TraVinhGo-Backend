// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.Features.Destination.Models;
public class TouristDestinationRequest
{
    public required string Name { get; set; }
    public double? AvarageRating { get; set; }
    public string? Description { get; set; }
    public required string Address { get; set; }
    public required Location Location { get; set; }
    public List<string>? Images { get; set; }
    public HistoryStory? HistoryStory { get; set; }
    public DateTime? UpdateAt { get; set; }
    public required string DestinationTypeId { get; set; }
    public OpeningHours? OpeningHours { get; set; }
    public string? Capacity { get; set; }
    public Contact? Contact { get; set; }
    public required string TagId { get; set; }
    public string? Ticket { get; set; }
    public int? TicketCount { get; set; }
}
