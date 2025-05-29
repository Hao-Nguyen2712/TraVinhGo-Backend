// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Application.Features.CommunityTips.Models;
public class CreateCommunityTipRequest
{
    [Required(ErrorMessage = "The Title is required.")]
    [MinLength(10, ErrorMessage = "Title must be at least 10 characters long.")]
    [MaxLength(100, ErrorMessage = "Title can be at most 100 characters long.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "The Content is required.")]
    [MinLength(10, ErrorMessage = "Content must be at least 10 characters long.")]
    [MaxLength(1000, ErrorMessage = "Content can be at most 1000 characters long.")]
    public string Content { get; set; }

    [Required(ErrorMessage = "The Tag is required.")]
    public string TagId { get; set; }

    public required bool Status { get; set; }
}
