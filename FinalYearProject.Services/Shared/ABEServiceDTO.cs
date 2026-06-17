using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FinalYearProject.Services.Shared;

public class PythonKeyResponse
{
    [JsonPropertyName("private_key")]
    public string PrivateKey { get; set; }
}