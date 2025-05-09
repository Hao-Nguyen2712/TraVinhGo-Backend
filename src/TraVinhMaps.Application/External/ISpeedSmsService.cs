// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TraVinhMaps.Application.Common.Dtos;

namespace TraVinhMaps.Application.External;

public interface ISpeedSmsService
{
    Task<SpeedSmsModel> sendSms(string[] phones, string content, int type, String sender);
    Task sendMMS(string[] phones, string content, string link, String sender);
}
