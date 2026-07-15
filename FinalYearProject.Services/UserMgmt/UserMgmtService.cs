

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FinalYearProject.Data.Context;
using FinalYearProject.Data.Domain.Config;
using FinalYearProject.Data.Domain.Entities;
using FinalYearProject.Data.Domain.Entities.Attributes;
using FinalYearProject.Data.Utilities;
using FinalYearProject.Services.EmailTransactionsMgmt;
using FinalYearProject.Services.EmailTransactionsMgmt.Templates.Auth;
using FinalYearProject.Services.Encryption;
using FinalYearProject.Services.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net.Http.Json;
using System.Text.Json;

namespace FinalYearProject.Services.UserMgmt;

public class UserMgmtService(
    FileSystemDbContext database,
    FileSystemConfig config,
    IEncryptionService encryptionService,
    IHttpClientFactory httpClientFactory,
    IAuthEmailTemplates authEmailTemplateService,
    IEmailTransactionsMgmtService emailService
) : IUserMgmtService
{
    private readonly Cloudinary _cloudinary =
    new Cloudinary(
        new Account(
            config.CloudinaryConfig.CloudName,
            config.CloudinaryConfig.ApiKey,
            config.CloudinaryConfig.ApiSecret
        )
    );

    public async Task<ServiceResponse<PaginationResponse<AllUsersResponse>>> GetUsersAsync(
    [FromQuery] UserParameters parameters,
    CancellationToken cancellationToken)
    {
        var query = database.Users
            .AsNoTracking()
            .Include(x => x.UsersAttributes)
            .ThenInclude(x => x.Attribute)
            .Where(x => !x.Deleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLower();

            query = query.Where(x =>
                x.FirstName.ToLower().Contains(search) ||
                x.LastName.ToLower().Contains(search) ||
                x.Email.ToLower().Contains(search));
        }

        if (parameters.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == parameters.IsActive.Value);
        }

        if (parameters.StartDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= parameters.StartDate.Value);
        }

        if (parameters.EndDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= parameters.EndDate.Value);
        }

        int totalRecords = await query.CountAsync(cancellationToken);

        int totalPages = (int)Math.Ceiling(
            (double)totalRecords / parameters.PageSize);

        var users = await query
            .OrderBy(x => x.CreatedAt)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        var response = users.Select(x => new AllUsersResponse(
            x.Id,
            x.FirstName,
            x.LastName,
            x.Email,
            x.ProfilePhotoUrl,
            x.IsActive
        )).ToList();

        return Response.Success(
            "Users retrieved successfully",
            new PaginationResponse<AllUsersResponse>(
                Records: response,
                TotalRecords: totalRecords,
                TotalPages: totalPages,
                CurrentPage: parameters.PageNumber,
                PageSize: parameters.PageSize
            ));
    }

    public async Task<ServiceResponse<UserResponse>> GetUserAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var user = await database.Users
            .Include(u => u.UsersAttributes)
                .ThenInclude(ua => ua.Attribute)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return Response.NotFound<UserResponse>("User not found", null!);

        var response = new UserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.ProfilePhotoUrl,
            user.UsersAttributes.Select(x => x.Attribute.AttributeName).ToList()
        );

        return Response.Success("User retrieved successfully", response);
    }

    public async Task<ServiceResponse> CreateUserAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Check email uniqueness
            bool emailExists = await database.Users
                .AnyAsync(u => u.Email == request.Email, cancellationToken);

            if (emailExists)
                return Response.Conflict("User already exists with this email");

            // 2. Validate attributes exist
            var attributes = await database.Attributes
                .Where(a => request.Attributes.Contains(a.Id))
                .ToListAsync(cancellationToken);

            var missingIds = request.Attributes.Except(attributes.Select(a => a.Id));

            if (missingIds.Any())
            {
                return Response.BadRequest("Invalid attribute(s) provided");
            }

            var attributeNames = attributes
                .Select(a => a.AttributeName)
                .ToArray();

            // 3. Call Python service to generate private key
            var encryptedPrivateKey =
            await GenerateEncryptedPrivateKeyAsync(
                attributeNames,
                cancellationToken);

            string autoGeneratedPassword = Password.Generate;

            var (publicId, photoUrl) = await UploadProfilePhotoAsync(request.ProfilePhoto, cancellationToken);

            // 4. Create user
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(autoGeneratedPassword),
                PrivateKey = encryptedPrivateKey,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                ProfilePhotoPublicId = publicId,
                ProfilePhotoUrl = photoUrl,
            };

            // 5. Create user-attribute relationships
            foreach (var attribute in attributes)
            {
                user.UsersAttributes.Add(new UserAttribute
                {
                    User = user,
                    AttributeId = attribute.Id,
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }

            database.Users.Add(user);

            SendEmailRequest emailRequest = new()
            {
                To = user.Email!,
                Subject = "Welcome to FileShare - Your Account Has Been Created",
                Body = authEmailTemplateService.AccountCreationTemplate(
                    user.FirstName,
                    user.Email,
                    autoGeneratedPassword,
                    config.EmailConfig.ProdURL
                 )
            };

            await emailService.SendEmailAsync(emailRequest, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);

            return Response.Created("User created successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating user");
            return Response.SystemMalfunction("Something went wrong");
        }
    }

    public async Task<ServiceResponse> UpdateUserAsync(
    Guid id,
    UpdateUserRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var user = await database.Users
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (user == null)
                return Response.NotFound("User not found");

            bool emailExists = await database.Users.AnyAsync(
                x => x.Email == request.Email && x.Id != id,
                cancellationToken);

            bool nameExists = await database.Users.AnyAsync(
                x => x.FirstName == request.FirstName && x.LastName == request.LastName && x.Id != id,
                cancellationToken);

            if (emailExists)
                return Response.Conflict("Another user already uses this email.");

            if (nameExists)
                return Response.Conflict("Another user already has this first and last name.");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;

            user.ModifiedAt = DateTimeOffset.UtcNow;

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;

            if (request.ProfilePhoto is not null)
            {
                // Delete previous profile picture if it exists
                if (!string.IsNullOrWhiteSpace(user.ProfilePhotoPublicId))
                {
                    await DeleteProfilePhotoAsync(user.ProfilePhotoPublicId);
                }

                var (publicId, photoUrl) =
                    await UploadProfilePhotoAsync(
                        request.ProfilePhoto,
                        cancellationToken);

                user.ProfilePhotoPublicId = publicId;
                user.ProfilePhotoUrl = photoUrl;
            }

            user.ModifiedAt = DateTimeOffset.UtcNow;

            await database.SaveChangesAsync(cancellationToken);

            return Response.Success("User updated successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating user");

            return Response.SystemMalfunction("Something went wrong.");
        }
    }

    public async Task<ServiceResponse> UpdateUserAttributesAsync(
    Guid id,
    UpdateUserAttributesRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var user = await database.Users
                .Include(x => x.UsersAttributes)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (user == null)
                return Response.NotFound("User not found");

            var attributes = await database.Attributes
                .Where(x => request.Attributes.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var missingIds = request.Attributes.Except(attributes.Select(x => x.Id));

            if (missingIds.Any())
                return Response.BadRequest("Invalid attribute(s) supplied.");


            database.Users_Attributes.RemoveRange(user.UsersAttributes);


            foreach (var attribute in attributes)
            {
                user.UsersAttributes.Add(new UserAttribute
                {
                    UserId = user.Id,
                    AttributeId = attribute.Id,
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }

            //--------------------------------------------------------
            // Generate NEW ABE private key
            //--------------------------------------------------------

            var attributeNames = attributes
                .Select(x => x.AttributeName)
                .ToArray();

            var encryptedPrivateKey =
           await GenerateEncryptedPrivateKeyAsync(
               attributeNames,
               cancellationToken);


            //--------------------------------------------------------
            // Encrypt new key
            //--------------------------------------------------------

            user.PrivateKey = encryptedPrivateKey;

            user.ModifiedAt = DateTimeOffset.UtcNow;

            await database.SaveChangesAsync(cancellationToken);

            return Response.Success("User attributes updated successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating user attributes.");
            return Response.SystemMalfunction("Something went wrong.");
        }
    }

    public async Task<ServiceResponse> UpdateUserStatusAsync(
    Guid id,
    UserStatusRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var user = await database.Users
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (user == null)
            {
                return Response.NotFound("User not found");
            }

            user.IsActive = request.IsActive;
            user.ModifiedAt = DateTimeOffset.UtcNow;

            await database.SaveChangesAsync(cancellationToken);

            return Response.Success(
                request.IsActive
                    ? "User activated successfully."
                    : "User deactivated successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating user status.");

            return Response.SystemMalfunction(
                "Something went wrong.");
        }
    }

    private async Task<string> GenerateEncryptedPrivateKeyAsync(
    IEnumerable<string> attributeNames,
    CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        var pythonResponse = await client.PostAsJsonAsync(
            config.ABEBaseURL + "/generate_key",
            new
            {
                attributes = attributeNames
            },
            cancellationToken);

        if (!pythonResponse.IsSuccessStatusCode)
        {
            throw new Exception("Failed to generate private key.");
        }

        var pythonResult = await pythonResponse.Content.ReadAsStringAsync();

        var keyResponse = JsonSerializer.Deserialize<PythonKeyResponse>(pythonResult);

        if (string.IsNullOrWhiteSpace(keyResponse?.PrivateKey))
        {
            throw new Exception("Python service returned an empty private key.");
        }

        return encryptionService.Encrypt(keyResponse.PrivateKey);
    }

    public async Task<ServiceResponse> DeleteUserAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        var user = await database.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return Response.NotFound("User not found");

        user.Deleted = true;
        user.IsActive = false;
        user.ModifiedAt = DateTimeOffset.UtcNow;


        await database.SaveChangesAsync(cancellationToken);

        return Response.Success("User deleted successfully");
    }

    private async Task<(string? PublicId, string? Url)> UploadProfilePhotoAsync(
    IFormFile? profilePhoto,
    CancellationToken cancellationToken)
    {
        if (profilePhoto is null || profilePhoto.Length == 0)
        {
            return (null, null);
        }

        await using var stream = profilePhoto.OpenReadStream();

        var uploadResult = await _cloudinary.UploadAsync(
            new ImageUploadParams
            {
                File = new FileDescription(
                    profilePhoto.FileName,
                    stream),

                Folder = "FileSharing/Profile_Photos"
            },
            cancellationToken);

        if (uploadResult.Error != null)
        {
            throw new InvalidOperationException(uploadResult.Error.Message);
        }

        return (
            uploadResult.PublicId,
            uploadResult.SecureUrl.ToString()
        );
    }

    private async Task DeleteProfilePhotoAsync(string? publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            return;

        await _cloudinary.DestroyAsync(
            new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            });
    }
}
