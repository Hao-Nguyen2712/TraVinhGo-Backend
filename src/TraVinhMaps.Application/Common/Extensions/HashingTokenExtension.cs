// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Cryptography;

namespace TraVinhMaps.Application.Common.Extensions;

public static class HashingTokenExtension
{
    public static string HashToken(string sessionId)
    {
        // Add a unique salt per session for better security
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Use a strong hashing algorithm like PBKDF2
        using var pbkdf2 = new Rfc2898DeriveBytes(sessionId, salt, 10000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);

        // Combine salt and hash for storage
        byte[] hashBytes = new byte[48];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }
}
