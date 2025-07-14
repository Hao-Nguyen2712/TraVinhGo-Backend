// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.Features.Translate.Models;

namespace TraVinhMaps.Application.Features.Translate.Interface;
public interface ITranslationService
{
    /// <summary>
    /// Translate <paramref name="text"/> from <paramref name="sourceLang"/> to <paramref name="targetLang"/>.
    /// </summary>
    Task<TranslationResult> TranslateAsync(string text, string sourceLang = "en", string targetLang = "vi", CancellationToken cancellationToken = default);
    Task<Dictionary<string, string>> TranslateBatchAsync(IEnumerable<string> texts, string sourceLang, string targetLang, CancellationToken ct);
}
