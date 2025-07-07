// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraVinhMaps.Application.Features.Translate.Models;
public class TranslationResult
{
    public string TranslatedText { get; set; } = default!;
    public bool FromCache { get; set; }
    public string ModelUsed { get; set; } = default!;
    public decimal? Cost { get; set; }
}
