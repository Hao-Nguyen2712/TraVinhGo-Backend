// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TraVinhMaps.Application.External;

namespace TraVinhMaps.Application.Features.Markers;
public class ImageManagementMarkerServices
{
    private readonly ICloudinaryService _cloudinaryService;

    public ImageManagementMarkerServices(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    public async Task<String> AddImageMarker(IFormFile file)
    {
        string result;
       
        if (file == null || file.Length == 0)
        {
            return null;
        }
        var imageResult = await this._cloudinaryService.UploadImageAsync(file);
        return imageResult.SecureUrl.ToString();  
    }

    public async Task<bool> DeleteImageMarker(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return false;

        try
        {
            var uri = new Uri(imageUrl);
            var fileName = uri.Segments.Last();
            var publicId = System.IO.Path.GetFileNameWithoutExtension(fileName);

            var result = await _cloudinaryService.DeleteImageAsync(publicId);
            return result.Result == "ok";
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
