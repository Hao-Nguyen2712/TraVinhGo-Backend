// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using TraVinhMaps.Application.Common.Exceptions;
using TraVinhMaps.Application.External;
using TraVinhMaps.Application.External.Models;

namespace TraVinhMaps.Infrastructure.External;

public class EmailSender : IEmailSender
{
    private readonly EmailConfiguration _emailConfig;

    public EmailSender(IOptions<EmailConfiguration> emailConfig)
    {
        _emailConfig = emailConfig.Value;

    }

    public async Task SendEmailAsync(string sendFor, string subject, string body, CancellationToken cancellationToken = default)
    {
        var email = _emailConfig.Email;
        var password = _emailConfig.Password;
        var host = _emailConfig.Host;
        var port = _emailConfig.Port;

        var smtpClient = new SmtpClient(host, port);
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;

        smtpClient.Credentials = new NetworkCredential(email, password);

        var bodyEmail = MailBodyForOTP(body);

        var message = new MailMessage(
            email!, sendFor, subject, bodyEmail
        )
        {
            IsBodyHtml = true,
        };

        try
        {
            await smtpClient.SendMailAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    private string MailBodyForOTP(string otp)
    {
        return $@"<html>
                    <body>
                        <h1>OTP Verification</h1>
                        <p>Your OTP is: {otp}</p>
                    </body>
                </html>";
    }
}
