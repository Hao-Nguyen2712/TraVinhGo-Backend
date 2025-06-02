// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TraVinhMaps.Application.Features.Markers.Models;
public class EditMarkerPictureRequest
{
    public string Id { get; set; }
    public IFormFile NewImageFile { get; set; }
    public string CurrentUrlImage { get; set; }
}
