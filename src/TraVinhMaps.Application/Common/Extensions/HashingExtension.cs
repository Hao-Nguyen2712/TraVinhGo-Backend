// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Cryptography;
using System.Text;

namespace TraVinhMaps.Application.Common.Extensions;

public static class HashingExtension
{

    /// <summary>
    /// Hashes the otp with SHA256 hashing.
    /// </summary>
    /// <param name="plainOtp">The plain otp.</param>
    /// <returns></returns>
    public static string HashWithSHA256(string plainOtp)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainOtp));

            // Convert byte array to a string (hexadecimal representation)
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2")); // "x2" for lowercase hex
            }
            return builder.ToString();
        }
    }
}
