// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Application.External.Models;

public class SpeedSmsSetting
{
    public string AccessToken { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.speedsms.vn/index.php";
}
