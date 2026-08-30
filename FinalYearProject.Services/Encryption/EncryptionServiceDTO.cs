using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FinalYearProject.Services.Encryption;

public class EncryptRequest
{
    public string Message { get; set; } = string.Empty;

    public string Policy { get; set; } = string.Empty;
}

public class CipherPayload
{
    [JsonPropertyName("nonce")]
    public string Nonce { get; set; } = string.Empty;

    [JsonPropertyName("cipher")]
    public string Cipher { get; set; } = string.Empty;

    [JsonPropertyName("tag")]
    public string Tag { get; set; } = string.Empty;
}

public class EncryptedFile
{
    public byte[] Cipher { get; set; } = default!;

    public byte[] Nonce { get; set; } = default!;

    public byte[] Tag { get; set; } = default!;
}