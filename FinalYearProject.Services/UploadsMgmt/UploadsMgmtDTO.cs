using FinalYearProject.Data.Utilities;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;


namespace FinalYearProject.Services.UploadsMgmt;

public class FileMgmtRequest
{
    public IFormFile File { get; set; } = default!;

    // Policy selected by user
    public Guid PolicyId { get; set; }
}

public class UpdateFilePolicyRequest
{
    public Guid PolicyId { get; set; }
}


public record FileResponse(
    Guid Id,
    string FileName,
    string ContentType,
    DateTimeOffset CreatedAt
    //Guid PolicyId,
    //string UploadedBy
);

public class FileParameters : RequestParameters
{
}

public record FileDetailsResponse(
    Guid Id,
    string FileName,
    string ContentType,
    string EncryptionPolicy,
    DateTimeOffset CreatedAt
    //string UploadedBy
);

public class FileDownloadResponse
{
    public byte[] FileBytes { get; set; }

    public string FileName { get; set; }

    public string ContentType { get; set; }
}

public class EncryptKeyRequest
{
    public string AesKey { get; set; } = default!;
    public string Policy { get; set; } = default!;
}

public class EncryptKeyResponse
{
    [JsonPropertyName("abe_ct")]
    public string Abe_Ct { get; set; } = default!;


    [JsonPropertyName("wrapped_key")]
    public string Wrapped_Key { get; set; } = default!;
}

public class DecryptKeyResponse
{
    [JsonPropertyName("aes_key")]
    public string Aes_Key { get; set; } = default!;
}
