// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Application.External;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;

    public ImageController(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(List<IFormFile> files)
    {
        if (files == null || !files.Any())
        {
            return BadRequest("No files uploaded.");
        }

        var response = new List<object>();

        try
        {
            foreach (var file in files)
            {
                var result = await _cloudinaryService.UploadImageAsync(file);
                response.Add(new
                {
                    PublicId = result.PublicId,
                    Url = result.Url?.ToString(),
                    SecureUrl = result.SecureUrl?.ToString()
                });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Upload failed: {ex.Message}");
        }
    }
}
