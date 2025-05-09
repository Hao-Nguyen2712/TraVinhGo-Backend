// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Application.Common.Dtos;

public class SpeedSmsModel
{
    public string Phone { get; set; }
    public string Content { get; set; }
    public int Type { get; set; }
    public string Sender { get; set; }
}
