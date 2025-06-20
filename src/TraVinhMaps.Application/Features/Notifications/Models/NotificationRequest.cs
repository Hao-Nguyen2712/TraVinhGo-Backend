// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraVinhMaps.Application.Features.Notifications.Models;
public class NotificationRequest
{
    //public required string UserId { get; set; }
    [Required(ErrorMessage = "Title cannot be empty.")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Title must be between 10 and 100 characters.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Content cannot be empty.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 1000 characters.")]
    public string Content { get; set; }

    public string IconCode { get; set; }

}
