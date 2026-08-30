using FinalYearProject.Data.Utilities;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;


namespace FinalYearProject.Services.UploadsMgmt;

public class FileMgmtRequest
{
    public IFormFile File { get; set; } = default!;

    // Policy selected by user
    public Guid PolicyId { get; set; }

    public bool? Vetted { get; set; } = false;
}

public class UpdateFilePolicyRequest
{
    public Guid PolicyId { get; set; }
}


public record FileResponse(
    Guid Id,
    string FileName,
    string ContentType,
    DateTimeOffset CreatedAt,
    string UploadedBy,
    Guid UploadedById
    //Guid PolicyId,
);

public class FileParameters : RequestParameters
{
    public Guid? UploadedBy { get; set; }
}

public record FileDetailsResponse(
    Guid Id,
    string FileName,
    string ContentType,
    string EncryptionPolicy,
    DateTimeOffset CreatedAt,
    Guid UploadedById
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
