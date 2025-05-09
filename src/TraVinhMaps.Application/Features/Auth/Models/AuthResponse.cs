// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Application.Features.Auth.Models;

/// <summary>
/// Response model for authentication requests
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the authentication was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the message describing the result
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets unique session ID for the user
    /// </summary>
    public string sessionId { get; set; }

    /// <summary>
    /// Gets or sets the refresh token for obtaining new access tokens
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the expiration time for the access token (in seconds)
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the user ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the user's role
    /// </summary>
    public string Role { get; set; }
}
