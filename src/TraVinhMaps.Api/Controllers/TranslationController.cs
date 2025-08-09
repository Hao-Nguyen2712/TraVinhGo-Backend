// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Application.External;
using TraVinhMaps.Application.Features.Translate.Interface;
using TraVinhMaps.Application.Features.Translate.Models;

namespace TraVinhMaps.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TranslationController : ControllerBase
{
    private readonly ITranslationService _translationService;
    private readonly ICacheService _cacheService;

    public TranslationController(ITranslationService translationService, ICacheService cacheService)
    {
        _translationService = translationService;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<IActionResult> Translate(string text, string sourceLang = "en", string targetLang = "vi", CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return BadRequest("Text is required");

        var cacheKey = $"{sourceLang}_{targetLang}_{text}";
        // Check if translation exists in cache
        var cachedTranslation = await _cacheService.GetData<TranslationResult>(cacheKey);
        if (cachedTranslation != null)
        {
            return Ok(cachedTranslation);
        }
        // If not in cache, call the translation service
        var result = await _translationService.TranslateAsync(text, sourceLang, targetLang, cancellationToken);

        await _cacheService.SetData(cacheKey, result, TimeSpan.FromMinutes(10));
        return Ok(result);
    }

    [HttpPost("batch")]
    public async Task<IActionResult> TranslateBatch(
        [FromBody] TranslationBatchRequest req,
        CancellationToken ct)
    {
        if (req.Texts == null || req.Texts.Count == 0)
            return BadRequest("No text");

        try
        {
            var distinctTexts = req.Texts.Distinct().ToList();
            var batchResult = await _translationService.TranslateBatchAsync(distinctTexts, req.SourceLang, req.TargetLang, ct);

            // Đảm bảo trả lại đúng key theo input order
            var result = new Dictionary<string, string>();
            foreach (var t in req.Texts)
            {
                result[t] = batchResult.TryGetValue(t, out var translated) ? translated : t;
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Batch translation failed: {ex.Message}");
        }
    }

    [HttpGet("cache")]
    public IActionResult GetCacheFile()
    {

        var path = Path.Combine(Directory.GetCurrentDirectory(), "Cache", "translations.json");
        if (!System.IO.File.Exists(path)) return NotFound("translations.json not found");
        var json = System.IO.File.ReadAllText(path);
        return Content(json, "application/json");
    }

}
