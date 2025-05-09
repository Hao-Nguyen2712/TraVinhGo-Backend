// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Application.Features.Auth.Models;

/// <summary>
/// Response model for OTP requests
/// </summary>
public class OtpResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the message describing the result
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the phone number that received the OTP
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the OTP expiration time in seconds
    /// </summary>
    public int ExpiresIn { get; set; } = 300; // Default 5 minutes

    /// <summary>
    /// Gets or sets the reference ID for the OTP request (for verification)
    /// </summary>
    public string ReferenceId { get; set; }
}
