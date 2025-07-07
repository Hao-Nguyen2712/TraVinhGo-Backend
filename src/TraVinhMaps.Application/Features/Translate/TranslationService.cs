// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Apis.Http;
using Microsoft.Extensions.Configuration;
using TraVinhMaps.Application.Features.Translate.Interface;
using TraVinhMaps.Application.Features.Translate.Models;

namespace TraVinhMaps.Application.Features.Translate;
public class TranslationService : ITranslationService
{
    private readonly HttpClient _httpClient;
    private readonly TranslationCache _cache;
    private readonly string _openAiKey;
    private const int MaxRetries = 3;
    private const int DelayMs = 1000;

    public TranslationService(IConfiguration configuration, System.Net.Http.IHttpClientFactory httpClientFactory)
    {
        _openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? configuration["OpenAi:ApiKey"];
        if (string.IsNullOrEmpty(_openAiKey))
            throw new ArgumentNullException("OpenAI API key is not configured.");

        _cache = new TranslationCache(Path.Combine("Cache", "translations.json"));
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiKey}");
        Directory.CreateDirectory(Path.GetDirectoryName(_cache.FilePath) ?? "Cache");
    }

    public async Task<TranslationResult> TranslateAsync(string text, string sourceLang = "vi", string targetLang = "en", CancellationToken cancellationToken = default)
    {
        // 1. Check cache
        var cached = _cache.Get(text, targetLang);
        if (!string.IsNullOrEmpty(cached))
        {
            return new TranslationResult
            {
                TranslatedText = cached,
                FromCache = true,
                ModelUsed = "cache",
                Cost = 0
            };
        }

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                // 2. Gọi OpenAI
                var prompt = $"Translate this from {sourceLang} to {targetLang}: {text}";
                var body = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                            new { role = "system", content = "You are a translation assistant." },
                            new { role = "user", content = prompt }
                        },
                    max_tokens = 100
                };
                var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content, cancellationToken);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(json);
                var translatedText = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()!.Trim();

                // 3. Lưu cache
                _cache.Set(text, targetLang, translatedText);

                return new TranslationResult
                {
                    TranslatedText = translatedText,
                    FromCache = false,
                    ModelUsed = "gpt-3.5-turbo",
                    Cost = null
                };
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests && attempt < MaxRetries)
            {
                Console.WriteLine($"Attempt {attempt} failed with 429. Retrying in {DelayMs}ms...");
                await Task.Delay(DelayMs, cancellationToken);
                continue;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Translation failed after {MaxRetries} attempts: {ex.Message}", ex);
            }
        }

        throw new Exception($"Translation failed after {MaxRetries} attempts.");
    }

    public class TranslationCache
    {
        public string FilePath { get; }
        private readonly object _lock = new();

        public TranslationCache(string filePath)
        {
            FilePath = filePath;
        }

        public string Get(string text, string targetLang)
        {
            lock (_lock)
            {
                if (File.Exists(FilePath))
                {
                    try
                    {
                        var dict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(FilePath));
                        if (dict != null && dict.TryGetValue(text, out var langs) && langs.TryGetValue(targetLang, out var result))
                            return result;
                    }
                    catch { /* handle if needed (log) */ }
                }
                return null;
            }
        }

        public void Set(string text, string targetLang, string translatedText)
        {
            lock (_lock)
            {
                Dictionary<string, Dictionary<string, string>> dict;
                if (File.Exists(FilePath))
                {
                    try
                    {
                        dict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(FilePath)) ?? new();
                    }
                    catch
                    {
                        dict = new();
                    }
                }
                else
                {
                    dict = new();
                }

                if (!dict.ContainsKey(text))
                    dict[text] = new();
                dict[text][targetLang] = translatedText;

                File.WriteAllText(FilePath, JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true }));
            }
        }
    }
}

