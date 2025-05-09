// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TraVinhMaps.Application.Common.Dtos;
using TraVinhMaps.Application.External;

namespace TraVinhMaps.Infrastructure.External;

public class SpeedSmsService : ISpeedSmsService
{

    public const int TYPE_QC = 1;
    public const int TYPE_CSKH = 2;
    public const int TYPE_BRANDNAME = 3;
    public const int TYPE_BRANDNAME_NOTIFY = 4; // Gửi sms sử dụng brandname Notify
    public const int TYPE_GATEWAY = 5; // Gửi sms sử dụng app android từ số di động cá nhân, download app tại đây: https://speedsms.vn/sms-gateway-service/

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SpeedSmsService> _logger;
    private const string _baseUrl = "https://api.speedsms.vn/index.php";
    private string acessToken = "VRM2fkngxgsw6sLiPDda5QvimGjdEDHa";


    public SpeedSmsService(IHttpClientFactory httpClientFactory, ILogger<SpeedSmsService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;

    }

    public Task sendMMS(string[] phones, string content, string link, string sender)
    {
        throw new NotImplementedException();
    }

    public async Task<SpeedSmsModel> sendSms(string[] phones, string content, int type, string sender)
    {
        string url = _baseUrl + "/sms/send";
        if (phones.Length <= 0)
        {
            return null;
        }
        if (String.IsNullOrEmpty(content))
        {
            return null;
        }
        if (type == TYPE_BRANDNAME && String.IsNullOrEmpty(sender))
        {
            return null;
        }
        var payload = new
        {
            to = phones,
            content = content,
            type = type,
            sender = sender
        };
        var json = JsonConvert.SerializeObject(payload);
        var client = _httpClientFactory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{acessToken}:x"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<SpeedSmsModel>(responseBody);
            return result;
        }
        else
        {
            _logger.LogError($"Error sending SMS: {response.StatusCode}");
            return null;
        }
    }
}
