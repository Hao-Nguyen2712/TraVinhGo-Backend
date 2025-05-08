// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace TraVinhMaps.Application.External;
public interface ICloudinaryService
{
    Task<ImageUploadResult> UploadImageAsync(IFormFile file);
    Task<DeletionResult> DeleteImageAsync(string publicId);
}
