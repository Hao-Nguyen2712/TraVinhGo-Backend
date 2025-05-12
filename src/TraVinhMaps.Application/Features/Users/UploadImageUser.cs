using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using TraVinhMaps.Application.External;

namespace TraVinhMaps.Application.Features.Users;

public class UploadImageUser
{
    private readonly ICloudinaryService _cloudinaryService;

    public UploadImageUser(ICloudinaryService cloudinaryService)
    {
        this._cloudinaryService = cloudinaryService;
    }

    public async Task<String> UploadImage(IFormFile file)
    {
        var imageResult = await this._cloudinaryService.UploadImageAsync(file);
        return imageResult.SecureUrl.ToString();
    }
}
