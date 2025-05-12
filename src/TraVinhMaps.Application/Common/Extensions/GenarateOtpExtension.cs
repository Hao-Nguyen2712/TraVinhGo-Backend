// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace TraVinhMaps.Application.Common.Extensions;

public static class GenarateOtpExtension
{
    public static string GenerateOtp()
    {
        var random = new Random();
        var otp = random.Next(100000, 999999).ToString(CultureInfo.InvariantCulture);
        return otp;
    }
}
