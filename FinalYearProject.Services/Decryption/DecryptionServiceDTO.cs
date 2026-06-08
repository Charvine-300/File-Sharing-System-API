using System;
using System.Collections.Generic;
using System.Text;

namespace FinalYearProject.Services.Decryption;

public class DecryptRequest
{
    public string CipherText { get; set; } = string.Empty;

    public string[] Attributes { get; set; } = [];
}