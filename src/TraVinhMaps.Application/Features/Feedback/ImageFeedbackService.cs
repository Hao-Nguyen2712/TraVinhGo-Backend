// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TraVinhMaps.Application.External;

namespace TraVinhMaps.Application.Features.Feedback;
public class ImageFeedbackService
{
    private readonly ICloudinaryService _cloudinaryService;

    public ImageFeedbackService(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }
    public async Task<List<string>> AddImageFeedback(List<IFormFile> files)
    {
        // Check number of image (maximum 3)
        if (files == null || files.Count == 0)
        {
            return new List<string>();
        }
        if (files.Count > 3)
        {
            throw new ArgumentException("Maximum of 3 images allowed.");
        }

        if (files == null || !files.Any())
        {
            return new List<string>(); // return list empty instead of null
        }

        var result = new List<string>();
        foreach (var fileItem in files)
        {
            if (fileItem == null || fileItem.Length == 0)
            {
                continue; // continue file empty
            } 
            // Only PNG and JPG files are allowed.
            var allowedContentTypes = new[] { "image/png", "image/jpeg" };
            if (!allowedContentTypes.Contains(fileItem.ContentType.ToLower()))
            {
                throw new ArgumentException("Only PNG and JPG images are allowed.");
            }

            using (var stream = fileItem.OpenReadStream())
            {
                var imageResult = await this._cloudinaryService.UploadImageAsync(fileItem);
                if (imageResult != null && !string.IsNullOrEmpty(imageResult.SecureUrl?.ToString()))
                {
                    result.Add(imageResult.SecureUrl.ToString());
                }
                else
                {
                    Console.WriteLine($"Failed to upload image: {fileItem.FileName}");
                }
            }
        }
        return result;
    }

    public async Task<bool> DeleteImageFeedback(string imageUrl)
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
