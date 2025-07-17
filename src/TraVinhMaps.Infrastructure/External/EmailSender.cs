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

    public async Task SendEmailPasswordAsync(string sendFor, string subject, string body, CancellationToken cancellationToken = default)
    {
        var email = _emailConfig.Email;
        var password = _emailConfig.Password;
        var host = _emailConfig.Host;
        var port = _emailConfig.Port;

        var smtpClient = new SmtpClient(host, port)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(email, password)
        };

        var parts = body.Split('|');
        var username = parts.ElementAtOrDefault(0) ?? "Admin";
        var emailAccount = parts.ElementAtOrDefault(1) ?? sendFor;
        var tempPassword = parts.ElementAtOrDefault(2) ?? "admin123@";

        var htmlBody = MailBodyForPassword(username, emailAccount, tempPassword);

        var message = new MailMessage(email!, sendFor, subject, htmlBody)
        {
            IsBodyHtml = true
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
        return $@"
    <html>
        <head>
            <style>
                .container {{
                    max-width: 600px;
                    margin: auto;
                    padding: 20px;
                    border: 1px solid #e0e0e0;
                    border-radius: 10px;
                    font-family: Arial, sans-serif;
                    background-color: #f9f9f9;
                    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
                }}
                .header {{
                    background-color: #4CAF50;
                    color: white;
                    padding: 10px 20px;
                    border-top-left-radius: 10px;
                    border-top-right-radius: 10px;
                    text-align: center;
                }}
                .content {{
                    padding: 20px;
                    text-align: center;
                }}
                .otp {{
                    font-size: 28px;
                    font-weight: bold;
                    color: #4CAF50;
                    margin: 20px 0;
                }}
                .footer {{
                    text-align: center;
                    font-size: 12px;
                    color: #777;
                    margin-top: 16px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h2>OTP Verification</h2>
                </div>
                <div class='content'>
                    <p>Thank you for using our service!</p>
                    <p>Please use the following One-Time Password (OTP) to continue:</p>
                    <div class='otp'>{otp}</div>
                    <p>This OTP is valid for 5 minutes. Please do not share it with anyone.</p>
                </div>
                <div class='footer'>
                    &copy; {DateTime.Now.Year} TraVinhGo. All rights reserved.
                </div>
            </div>
        </body>
    </html>";
    }
    private string MailBodyForPassword(string username, string email, string tempPassword)
    {
        return $@"
    <html>
        <head>
            <style>
                .container {{
                    max-width: 600px;
                    margin: auto;
                    padding: 20px;
                    border: 1px solid #e0e0e0;
                    border-radius: 10px;
                    font-family: Arial, sans-serif;
                    background-color: #f9f9f9;
                    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
                }}
                .header {{
                    background-color: #4CAF50;
                    color: white;
                    padding: 10px 20px;
                    border-top-left-radius: 10px;
                    border-top-right-radius: 10px;
                    text-align: center;
                }}
                .content {{
                    padding: 20px;
                    line-height: 1.6;
                    color: #4CAF50;
                }}
                .password-box {{
                    font-size: 20px;
                    font-weight: bold;
                    color: #D32F2F;
                    background-color: #fbe9e7;
                    padding: 10px;
                    border-radius: 8px;
                    margin: 15px 0;
                    word-break: break-all;
                    text-align: center;
                }}
                .footer {{
                    text-align: center;
                    font-size: 12px;
                    color: #777;
                    margin-top: 16px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h2>Welcome to TraVinhMaps</h2>
                </div>
                <div class='content'>
                    <p>Hello <strong>{username}</strong>,</p>
                    <p>Your administrator account has been successfully created.</p>
                    <p><strong>Email:</strong> {email}</p>
                    <p><strong>Temporary Password:</strong></p>
                    <div class='password-box'>{tempPassword}</div>
                    <p><strong>Note: ⚠️ For your security, please log in and change your password immediately.⚠️</strong></p>
                    <p>Thank you,<br/>TraVinhGo Team</p>
                </div>
                <div class='footer'>
                    &copy; {DateTime.Now.Year} TraVinhGo. All rights reserved.
                </div>
            </div>
        </body>
    </html>";
    }

}
