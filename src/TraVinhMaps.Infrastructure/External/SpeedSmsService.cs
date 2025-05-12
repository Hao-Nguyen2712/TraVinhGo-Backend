// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TraVinhMaps.Application.External;
using TraVinhMaps.Application.External.Models;

namespace TraVinhMaps.Infrastructure.External;

public class SpeedSmsService : ISpeedSmsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SpeedSmsService> _logger;
    private readonly SpeedSmsSetting _speedSmsSetting;

    public SpeedSmsService(
        IHttpClientFactory httpClientFactory,
        ILogger<SpeedSmsService> logger,
        IOptions<SpeedSmsSetting> speedSmsSetting)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _speedSmsSetting = speedSmsSetting.Value;
    }

    public async Task SendSMS(string phoneTo, string message)
    {
        var formattedPhone = FormatPhoneNumber(phoneTo);
        var url = $"{_speedSmsSetting.BaseUrl}/sms/send?content={Uri.EscapeDataString(message)}&sender={_speedSmsSetting.DeviceId}&to={formattedPhone}";
        var client = _httpClientFactory.CreateClient();
        var authValue = EncodeToBase64(_speedSmsSetting.AccessToken);
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);
        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"SMS sent successfully: {responseBody}");
        }
        else
        {
            _logger.LogError($"Error sending SMS: {response.StatusCode}");
        }
    }

    private string FormatPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            throw new ArgumentException("Phone number cannot be null or empty", nameof(phoneNumber));
        }

        // Check if the phone number starts with '0'
        if (phoneNumber.StartsWith("0"))
        {
            // Replace the leading '0' with '84'
            return "84" + phoneNumber.Substring(1);
        }

        // Return the original phone number if it doesn't start with '0'
        return phoneNumber;
    }

    private string EncodeToBase64(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return string.Empty;
        }

        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(plainText + ":x");
        return Convert.ToBase64String(textBytes);
    }
}
