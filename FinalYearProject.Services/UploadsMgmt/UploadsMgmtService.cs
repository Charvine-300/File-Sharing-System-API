using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FinalYearProject.Data.Context;
using FinalYearProject.Data.Domain.Config;
using FinalYearProject.Data.Domain.Entities;
using FinalYearProject.Data.Domain.Entities.Shared;
using FinalYearProject.Data.Utilities;
using FinalYearProject.Services.AuditTrails;
using FinalYearProject.Services.Encryption;
using FinalYearProject.Services.Shared;
using FinalYearProject.Services.Shared.PolicyAuthorizationService;
using FinalYearProject.Services.Shared.UserContextService;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using Log = Serilog.Log;

namespace FinalYearProject.Services.UploadsMgmt;

public class UploadsMgmtService(
    FileSystemDbContext database,
    FileSystemConfig config,
    IEncryptionService encryptionService,
    IHttpClientFactory httpClientFactory,
    IUserContextService userContextService,
    IAuditLogMgmtService auditLogService,
    IPolicyAuthorizationService policyAuthorizationService
) : IUploadsMgmtService
{
    private readonly HttpClient _client = httpClientFactory.CreateClient();

    private readonly Cloudinary _cloudinary =
    new Cloudinary(
        new Account(
            config.CloudinaryConfig.CloudName,
            config.CloudinaryConfig.ApiKey,
            config.CloudinaryConfig.ApiSecret
        )
    );


    public async Task<ServiceResponse<PaginationResponse<FileResponse>>> GetFilesAsync(
    FileParameters parameters,
    CancellationToken cancellationToken)
    {
        try
        {
            var query = database.Uploads
                .Include(x => x.User)
                .AsQueryable();


            // Search filter
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                query = query.Where(x =>
                    x.Filename.Contains(parameters.Search));
            }

            if (parameters.UploadedBy.HasValue)
            {
                query = query.Where(x =>
                    x.UserId == parameters.UploadedBy.Value);
            }


            // Total count before pagination
            var totalRecords = await query.CountAsync(cancellationToken);


            var files = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(
                    (parameters.PageNumber - 1) *
                    parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);


            var data = files.Select(a => new FileResponse(
                a.Id,
                a.Filename,
                a.ContentType,
                a.CreatedAt,
                a.CreatedBy ?? "Unknown",
                a.UserId
            )).ToList();


            var response = new PaginationResponse<FileResponse>(
                Records: data,
                TotalRecords: totalRecords,
                TotalPages: (int)Math.Ceiling(totalRecords / (double)parameters.PageSize),
                CurrentPage: parameters.PageNumber,
                PageSize: parameters.PageSize
            );


            return Response.Success("Files fetched successfully", response);
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "Error fetching uploaded files");
            return Response.SystemMalfunction<
                PaginationResponse<FileResponse>>(
                    "Something went wrong", null!);
        }
    }

    public async Task<ServiceResponse<FileDetailsResponse>> GetFileAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        try
        {
            var upload =
                await database.Uploads
                .Include(x => x.User)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);


            if (upload == null)
            {
                return Response.NotFound<FileDetailsResponse>(
                    "File not found", null!);
            }


            var response = new FileDetailsResponse(
                 upload.Id,
                 upload.Filename,
                 upload.ContentType,
                 upload.Policy?.PolicyName ?? "No policy found",
                 upload.CreatedAt,
                 upload.UserId
             );


            return Response.Success("File details fetch successfully", response);
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "Error retrieving file {FileId}",
                id);

            return Response.SystemMalfunction<
                FileDetailsResponse>(
                    "Something went wrong", null!);
        }
    }

    public async Task<ServiceResponse> UploadFileAsync(FileMgmtRequest request, CancellationToken cancellationToken)
    {
        var policy = await database.Policies
            .FirstOrDefaultAsync(
                p => p.Id == request.PolicyId,
                cancellationToken);

        if (policy == null)
        {
            return Response.NotFound("Policy not found");
        }


        if (request?.Vetted == false)
        {

            var isSuperAdmin = userContextService.User.UserType == UserType.SuperAdmin;
            var currentUser = await database.Users
                .Include(u => u.UsersAttributes)
                .FirstOrDefaultAsync(u => u.Id == userContextService.User.Id, cancellationToken);

            var authorizationResult =
                policyAuthorizationService.CanBypassPolicy(
                    policy.PolicyExpression,
                    currentUser.UsersAttributes
                        .Select(x => x.AttributeId),
                    isSuperAdmin);

            if (!authorizationResult.IsAuthorized)
            {
                return Response.Forbidden(
                    authorizationResult.FailureReason);
            }
        }

        // Read the file into a byte array
        await using var stream = request.File.OpenReadStream();

        using var ms = new MemoryStream();

        await stream.CopyToAsync(ms, cancellationToken);

        byte[] fileBytes = ms.ToArray();


        // Generate a random AES key
        byte[] aesKeyBytes = RandomNumberGenerator.GetBytes(32);

        string aesKey =
            Convert.ToBase64String(aesKeyBytes);

        EncryptedFile encryptedFile =
            encryptionService.EncryptFile(
                fileBytes,
                aesKeyBytes);

        string encryptionPolicy =
            BuildEncryptionPolicy(policy.PolicyExpression); 

        // Encrypt AES Key in CP-ABE 
        var pythonResponse =
            await _client.PostAsJsonAsync(
                $"{config.ABEBaseURL}/encrypt_key",
                new
                {
                    aes_key = aesKey,
                    policy = encryptionPolicy
                });

        if (!pythonResponse.IsSuccessStatusCode)
        {
            return Response.BadRequest(
                "Unable to encrypt AES key.");
        }


        var wrappedKey =
            JsonSerializer.Deserialize<EncryptKeyResponse>(
                await pythonResponse.Content.ReadAsStringAsync());

        // Upload the encrypted file to Cloudinary
        var uploadResult = _cloudinary.Upload(
            new RawUploadParams
            {
                File = new FileDescription(
                    request.File.FileName,
                    new MemoryStream(encryptedFile.Cipher)
                ),

                Folder = "FileSharing"

                // TODO:
                // move files into user folders
                // once authentication is implemented
            });

        Upload upload = new()
        {
            UserId = userContextService.User.Id,

            Filename = request.File.FileName,

            ContentType = request.File.ContentType,

            CloudinaryPublicId = uploadResult.PublicId,

            CloudinaryUrl = uploadResult.SecureUrl.ToString(),

            AbeCipherText = wrappedKey.Abe_Ct,

            WrappedKey = wrappedKey.Wrapped_Key,

            PolicyId = request.PolicyId,

            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = userContextService.User.FirstName,

            FileNonce = Convert.ToBase64String(encryptedFile.Nonce),

            FileTag = Convert.ToBase64String(encryptedFile.Tag)
        };


        await auditLogService.CreateAuditLogAsync(
            new CreateAuditTrailRequest
            {
                Action = "File Upload",
                Description = $"{userContextService.User.FirstName} {userContextService.User.LastName} uploaded a file `{request.File.FileName}` with the `{policy.PolicyName}` policy",
                Actor = $"{userContextService.User.FirstName} {userContextService.User.LastName}",
                ActionType = ActionType.Create
            },
            cancellationToken
        );

        database.Uploads.Add(upload);

        await database.SaveChangesAsync(cancellationToken);

        return Response.Created("File uploaded successfully");
    }

    public async Task<ServiceResponse<FileDownloadResponse>> DownloadFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        var upload = await database.Uploads
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.Id == fileId,
                cancellationToken);

        var user = await database.Users
            .FirstOrDefaultAsync(
                x => x.Id == userContextService.User.Id,
                cancellationToken);

        if (user == null)
        {
            return Response.NotFound<FileDownloadResponse>("User not found", null!);
        }


        string privateKey =
            encryptionService.Decrypt(
                user.PrivateKey);

        var response =
            await _client.PostAsJsonAsync(
                $"{config.ABEBaseURL}/decrypt_key",
                new
                {
                    private_key = privateKey,

                    cipher = new
                    {
                        abe_ct = upload.AbeCipherText,
                        wrapped_key = upload.WrappedKey
                    }
                });

        if (!response.IsSuccessStatusCode)
        {
            return Response.Forbidden<FileDownloadResponse>(
                "You do not possess the attributes required to access this file", null!);
        }

        var keyResponse =
            JsonSerializer.Deserialize<DecryptKeyResponse>(
                await response.Content.ReadAsStringAsync());


        string aesKey =
            keyResponse.Aes_Key;

        var encryptedBytes =
            await _client.GetByteArrayAsync(
                upload.CloudinaryUrl);

        var encryptedFile = new EncryptedFile
        {
            Cipher = encryptedBytes,
            Nonce = Convert.FromBase64String(upload.FileNonce),
            Tag = Convert.FromBase64String(upload.FileTag)
        };

        var plaintext =
            encryptionService.DecryptFile(
                encryptedFile,
                Convert.FromBase64String(aesKey));

        await auditLogService.CreateAuditLogAsync(
            new CreateAuditTrailRequest
            {
                Action = "File Download",
                Description = $"{userContextService.User.FirstName} {userContextService.User.LastName} downloaded a file `{upload.Filename}`",
                Actor = $"{userContextService.User.FirstName} {userContextService.User.LastName}",
                ActionType = ActionType.Other
            },
            cancellationToken
        );

        await database.SaveChangesAsync(cancellationToken);
        return Response.Success<FileDownloadResponse>("File downloaded successfully", new FileDownloadResponse
        {
            FileBytes = plaintext,
            FileName = upload.Filename,
            ContentType = upload.ContentType
        });
    }

    public async Task<ServiceResponse> UpdateFilePolicyAsync(
    Guid id,
    UpdateFilePolicyRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var upload = await database.Uploads
                .Include(x => x.Policy)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (upload == null)
                return Response.NotFound("File not found.");

            if (upload.UserId != userContextService.User.Id &&
                userContextService.User.UserType != UserType.SuperAdmin)
            {
                return Response.Forbidden(
                    "You are not allowed to update this file.");
            }

            var newPolicy = await database.Policies
                .FirstOrDefaultAsync(
                    p => p.Id == request.PolicyId,
                    cancellationToken);

            if (newPolicy == null)
                return Response.NotFound("Policy not found.");

            if (upload.PolicyId == request.PolicyId)
                return Response.BadRequest(
                    "The file is already using this policy.");

            var user = await database.Users
                .FirstOrDefaultAsync(
                    x => x.Id == userContextService.User.Id,
                    cancellationToken);

            if (user == null)
                return Response.NotFound("User not found.");

            string privateKey =
                encryptionService.Decrypt(user.PrivateKey);

            // Recover the AES key using the current policy
            var decryptResponse =
                await _client.PostAsJsonAsync(
                    $"{config.ABEBaseURL}/decrypt_key",
                    new
                    {
                        private_key = privateKey,
                        cipher = new
                        {
                            abe_ct = upload.AbeCipherText,
                            wrapped_key = upload.WrappedKey
                        }
                    });

            if (!decryptResponse.IsSuccessStatusCode)
            {
                return Response.Forbidden(
                    "You are not authorized to update this file's policy.");
            }

            var aesResponse =
                JsonSerializer.Deserialize<DecryptKeyResponse>(
                    await decryptResponse.Content.ReadAsStringAsync());

            string encryptionPolicy =
                BuildEncryptionPolicy(newPolicy.PolicyExpression);

            // Encrypt the SAME AES key using the NEW policy
            var encryptResponse =
                await _client.PostAsJsonAsync(
                    $"{config.ABEBaseURL}/encrypt_key",
                    new
                    {
                        aes_key = aesResponse.Aes_Key,
                        policy = encryptionPolicy
                    });

            if (!encryptResponse.IsSuccessStatusCode)
            {
                return Response.BadRequest(
                    "Unable to encrypt AES key with the new policy.");
            }

            var wrappedKey =
                JsonSerializer.Deserialize<EncryptKeyResponse>(
                    await encryptResponse.Content.ReadAsStringAsync());

            upload.PolicyId = newPolicy.Id;
            upload.AbeCipherText = wrappedKey.Abe_Ct;
            upload.WrappedKey = wrappedKey.Wrapped_Key;
            upload.ModifiedAt = DateTimeOffset.UtcNow;
            upload.ModifiedBy = userContextService.User.FirstName;

            await auditLogService.CreateAuditLogAsync(
                new CreateAuditTrailRequest
                {
                    Action = "File Policy Update",
                    Description = $"{userContextService.User.FirstName} {userContextService.User.LastName} updated the policy for file {upload.Filename}",
                    Actor = $"{userContextService.User.FirstName} {userContextService.User.LastName}",
                    ActionType = ActionType.Update
                },
                cancellationToken
            );

            await database.SaveChangesAsync(cancellationToken);

            return Response.Success(
                "File policy updated successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating file policy.");

            return Response.SystemMalfunction(
                "Something went wrong.");
        }
    }

    public async Task<ServiceResponse> DeleteFileAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        try
        {
            var upload =
                await database.Uploads
                    .FirstOrDefaultAsync(
                        x => x.Id == id,
                        cancellationToken);


            if (upload == null)
            {
                return Response.NotFound(
                    "File not found");
            }


            // Delete from Cloudinary
            var deletionResult =
                await _cloudinary.DestroyAsync(
                    new DeletionParams(
                        upload.CloudinaryPublicId)
                    {
                        ResourceType =
                            ResourceType.Raw
                    });


            if (deletionResult.Result != "ok")
            {
                Log.Warning(
                    "Cloudinary deletion failed for {PublicId}",
                    upload.CloudinaryPublicId);
            }


            await auditLogService.CreateAuditLogAsync(
                new CreateAuditTrailRequest
                {
                    Action = "File Deletion",
                    Description = $"{userContextService.User.FirstName} {userContextService.User.LastName} deleted file {upload.Filename}",
                    Actor = $"{userContextService.User.FirstName} {userContextService.User.LastName}",
                    ActionType = ActionType.Delete
                },
                cancellationToken
            );

            // Delete DB record
            database.Uploads.Remove(upload);

            await database.SaveChangesAsync(
                cancellationToken);


            return Response.Success(
                "File deleted successfully");
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "Error deleting file {FileId}",
                id);

            return Response.SystemMalfunction(
                "Something went wrong");
        }
    }

    private string BuildEncryptionPolicy(string policyExpression)
    {
        return $"({policyExpression} or SuperAdmin)";
    }
}
