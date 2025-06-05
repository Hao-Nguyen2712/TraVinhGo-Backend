// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.ComponentModel.DataAnnotations;

namespace TraVinhMaps.Application.Features.LocalSpecialties.Models;
public class CreateLocalSpecialtiesRequest
{
    [Required(ErrorMessage = "Food name is required.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Food name must be between 5 and 100 characters.")]
    public string FoodName { get; set; } = default!;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(3000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 3000 characters.")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "TagId is required.")]
    public string TagId { get; set; } = default!;
    public bool Status { get; set; } = true;
}
