using System;
using System.Collections.Generic;
using System.Text;

namespace FinalYearProject.Services.Encryption;

public class EncryptRequest
{
    public string Message { get; set; } = string.Empty;

    public string Policy { get; set; } = string.Empty;
}

public class CipherPayload
{
    public string Nonce { get; set; } = default!;
    public string Cipher { get; set; } = default!;
    public string Tag { get; set; } = default!;
}