// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Application.External.Models;

public class EmailConfiguration
{
    public string Host { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Email { get; set; } = string.Empty;
}
