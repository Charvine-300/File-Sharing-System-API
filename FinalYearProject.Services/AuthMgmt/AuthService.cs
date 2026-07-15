

using FinalYearProject.Data.Context;
using FinalYearProject.Data.Domain.Config;
using FinalYearProject.Data.Domain.Entities;
using FinalYearProject.Data.Domain.Entities.Shared;
using FinalYearProject.Data.Extensions;
using FinalYearProject.Data.Utilities;
using FinalYearProject.Services.Shared;
using FinalYearProject.Services.Shared.Security;
using FinalYearProject.Services.Shared.UserContextService;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FinalYearProject.Services.AuthMgmt;

public class AuthService(FileSystemDbContext database, FileSystemConfig config, IUserContextService userContextService) : IAuthService
{
    private readonly int RefreshTokenValidity = 7;
    public async Task<ServiceResponse<LoginResponse>> LoginAsync(
    LoginRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            string email = request.Email.Trim();
            string password = request.Password.Trim();

                User? user = await database.Users.AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.Email == email,
                        cancellationToken);


            if (user is null)
            {
                return Response.Unauthorized<LoginResponse>(
                    "Invalid login credentials",
                    null!);
            }

            bool isCorrectPassword = BCryptHash.Verify(user.Password!, password);

            if (!isCorrectPassword)
            {
                return Response.BadRequest<LoginResponse>(
                    "Incorrect email address or password",
                    null!);
            }

            if (!user.IsActive)
            {
                return Response.Forbidden<LoginResponse>(
                    "Your account is not active. Please contact support.",
                    null!);
            }

            LoginResponse result = await CreateAccessTokenAsync(
                user,
                true,
                cancellationToken);

            return Response.Success("Login successful.", result);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);

            return Response.SystemMalfunction<LoginResponse>(
                "Login failed. It is not you, it's us. Please try again or contact support.",
                null!);
        }
    }

    protected async Task<LoginResponse> CreateAccessTokenAsync(User? user, bool isLogin, CancellationToken cancellationToken)
    {
        if (user == null)
        {
            throw new AuthenticationException("Invalid login credentials");
        }

        JwtSecurityTokenHandler jwTokenHandler = new();
        string? secretKey = config.JwtConfig.JwtKey;
        byte[] key = Encoding.ASCII.GetBytes(secretKey);
        UserType userType = user.UserType;


        Claim[] claims = [
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("id", user.Id.ToString()),
                new Claim("userType",userType.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(
    JwtRegisteredClaimNames.Iat,
    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
    ClaimValueTypes.Integer64
)
        ];

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims = [..claims,
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                ];

        }


        //if (user is CorporateUser || user is BackOfficeUser)
        //{
        //    string[] privileges = await GetUserRolePrivilegesAsync(roles, user is CorporateUser);
        //    foreach (string privilege in privileges)
        //    {
        //        claims = [.. claims, new Claim("permission", privilege)];
        //    }
        //}

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(config.JwtConfig.JwtExpireMinutes),
            Audience = config.JwtConfig.JwtAudience,
            Issuer = config.JwtConfig.JwtIssuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        SecurityToken securityToken = jwTokenHandler.CreateToken(tokenDescriptor);
        string accessToken = jwTokenHandler.WriteToken(securityToken);
        string refreshToken = await GenerateRefreshTokenAsync(user.Id, user.UserType);

        await database.SaveChangesAsync(cancellationToken);

        return new LoginResponse(
            accessToken,
            refreshToken,
            tokenDescriptor.Expires.Value,
            user.FirstName,
            user.LastName,
            userType.ToString(),
            user.Id,
            user.ProfilePhotoUrl
        );
    }

    private async Task<string> GenerateRefreshTokenAsync(Guid userId, UserType userType)
    {
        byte[] randomNumber = new byte[32];

        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(randomNumber);
            string refreshToken = Convert.ToBase64String(randomNumber);

            IQueryable<RefreshToken> query = database.RefreshTokens
                .Where(x => x.IsActive);

       
            query = query.Where(x => x.Id == userId);

            List<RefreshToken> refreshTokens = await query.ToListAsync();

            foreach (var token in refreshTokens)
            {
                token.IsActive = false;
                token.UpdatedAt = DateTime.UtcNow;
            }

            // 🔥 Create new token with correct FK
            RefreshToken newRefreshToken = new()
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenValidity),
                IsActive = true,
                Token = refreshToken,
                UserId = userId
            };

            await database.RefreshTokens.AddAsync(newRefreshToken);

            return refreshToken;
        }
    }

    public async Task<ServiceResponse> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        User? userDetails = null;
 
            userDetails = await database.Users.FirstOrDefaultAsync(x => x.Id == userContextService.User.Id, cancellationToken);
     
        if (userDetails is null)
        {
            return Response.NotFound("User does not exist");
        }



        bool isValidPin = request.OldPassword.Verify(userDetails.Password!);
        if (!isValidPin)
        {
            return Response.BadRequest("Your current password does not match.");
        }

        bool isNotNewPassword = request.NewPassword.Verify(userDetails.Password);
        if (isNotNewPassword)
        {
            return Response.BadRequest("You cannot use your old password.");
        }

        userDetails.ModifiedAt = DateTimeOffset.UtcNow;
        userDetails.ModifiedBy = userContextService.User.FirstName;
        userDetails.Password = request.NewPassword.Hash();

        await database.SaveChangesAsync(cancellationToken);
        return Response.Success("Password updated successfully");
    }
}
