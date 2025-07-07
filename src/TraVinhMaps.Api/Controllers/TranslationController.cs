// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Application.Features.Translate.Interface;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TranslationController : ControllerBase
{
    private readonly ITranslationService _translationService;

    public TranslationController(ITranslationService translationService)
    {
        _translationService = translationService;
    }

    [HttpGet]
    public async Task<IActionResult> Translate([FromQuery] string text, [FromQuery] string sourceLang = "en", [FromQuery] string targetLang = "vi")
    {
        if (string.IsNullOrWhiteSpace(text)) return BadRequest("Missing text");
        var result = await _translationService.TranslateAsync(text, sourceLang, targetLang);
        return Ok(result);
    }
}
