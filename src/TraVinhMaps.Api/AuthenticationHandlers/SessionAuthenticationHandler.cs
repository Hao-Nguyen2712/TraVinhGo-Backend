// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using TraVinhMaps.Application.Common.Extensions;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Api.AuthenticationHandlers;

public class SessionAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    // The header name to look for the session ID
    public string HeaderName { get; set; } = "sessionId";

    // Whether to validate session expiration
    public bool ValidateExpiration { get; set; } = true;

    // Controls behavior when authentication fails
    public bool SuppressWWWAuthenticateHeader { get; set; } = false;

    // Custom response for authentication challenges
    public string Challenge { get; set; } = "Session";

    // Maximum session age in minutes (optional additional check)
    public int MaximumSessionAge { get; set; } = 1440; // 24 hours

}

public class SessionAuthenticationHandler : AuthenticationHandler<SessionAuthenticationSchemeOptions>
{
    private readonly IRepository<UserSession> _sessionRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Role> _roleRepository;

    public SessionAuthenticationHandler(
        IOptionsMonitor<SessionAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IRepository<UserSession> sessionRepository,
        IRepository<User> userRepository,
        IRepository<Role> roleRepository)
        : base(options, logger, encoder)
    {
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // 1. Check if the "sessionId" header exists.
        if (!Request.Headers.TryGetValue("sessionId", out var sessionIdValues))
        {
            return AuthenticateResult.NoResult(); // No header, so no authentication attempt.
        }

        var sessionId = sessionIdValues.FirstOrDefault();
        if (string.IsNullOrEmpty(sessionId))
        {
            return AuthenticateResult.Fail("Session ID header is present but empty.");
        }

        // 2. Hash the incoming sessionId to match the stored hash.
        // Assuming HashingExtension.HashWithSHA256 is the correct method used for SessionId
        var hashedSessionId = HashingTokenExtension.HashToken(sessionId);

        // 3. Validate the sessionId against the UserSession entity.
        var userSession = await _sessionRepository.GetAsyns(x => x.SessionId == hashedSessionId);

        if (userSession == null)
        {
            return AuthenticateResult.Fail("Invalid or expired session ID.");
        }

        if (Options.ValidateExpiration && userSession.RefreshTokenExpireAt < DateTime.UtcNow)
        {
            userSession.IsActive = false; // Mark session as inactive if expired
            await _sessionRepository.UpdateAsync(userSession, System.Threading.CancellationToken.None);
            return AuthenticateResult.Fail("Session ID has expired.");
        }

        if (!userSession.IsActive)
        {
            return AuthenticateResult.Fail("Session ID is not active.");
        }

        var user = await _userRepository.GetByIdAsync(userSession.UserId, System.Threading.CancellationToken.None);
        if (user == null)
        {
            return AuthenticateResult.Fail("User associated with the session not found.");
        }

        var role = await _roleRepository.GetByIdAsync(user.RoleId, System.Threading.CancellationToken.None);
        var roleName = role?.RoleName ?? "user"; // Default to "user" if role not found or RoleName is null

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Email ?? user.PhoneNumber ?? "N/A"), // Use email or phone as name
            new Claim(ClaimTypes.Role, roleName),
            new Claim("sessionId", sessionId) // Optionally include the original session ID as a claim
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

}
