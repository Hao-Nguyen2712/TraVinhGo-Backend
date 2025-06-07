// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TraVinhMaps.Application.External;
using TraVinhMaps.Application.External.Models;

namespace TraVinhMaps.Infrastructure.External;
public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySetting> config)
    {
        var acc = new Account
               (
               config.Value.CloudName,
               config.Value.ApiKey,
               config.Value.ApiSecret
               );
        _cloudinary = new Cloudinary(acc);
    }

    public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is null or empty", nameof(file));
        }

        if (file.Length > 0)
        {
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream)
            };
            try
            {
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new InvalidOperationException("Error uploading image to Cloudinary", ex);
            }
        }
        return uploadResult;
    }

    public Task<DeletionResult> DeleteImageAsync(string publicId)
    {
        var deleteResult = new DeletionParams(publicId);
        var result = _cloudinary.DestroyAsync(deleteResult);
        return result;
    }
}
