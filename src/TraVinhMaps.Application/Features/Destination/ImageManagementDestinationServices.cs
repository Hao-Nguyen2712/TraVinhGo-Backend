// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TraVinhMaps.Application.External;

namespace TraVinhMaps.Application.Features.Destination;
public class ImageManagementDestinationServices
{
    private readonly ICloudinaryService _cloudinaryService;

    public ImageManagementDestinationServices(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    public async Task<List<String>> AddImageDestination(List<IFormFile> file)
    {
        List<String> result = new List<String>();
        //var imageResult = await this._cloudinaryService.UploadImageAsync(file);
        foreach (IFormFile fileItem in file)
        {
            if (fileItem.Length == 0 || file.Count == 0)
            {
                return null;
            }
            using (var stream = fileItem.OpenReadStream())
            {
                var imageResult = await this._cloudinaryService.UploadImageAsync(fileItem);
                result.Add(imageResult.SecureUrl.ToString());
            }
        }
        //return imageResult.SecureUrl.ToString();
        return result;
    }

    public async Task<bool> DeleteImageDestination(string imageUrl)
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
