// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraVinhMaps.Application.Features.Translate.Models;
public class TranslationBatchRequest
{
    public List<string> Texts { get; set; } = new();
    public string SourceLang { get; set; } = "auto";
    public string TargetLang { get; set; } = "vi";
}
