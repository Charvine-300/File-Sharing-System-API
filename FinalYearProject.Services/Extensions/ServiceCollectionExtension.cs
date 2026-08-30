using FinalYearProject.Data.Context;
using FinalYearProject.Data.Domain.Config;
using FinalYearProject.Services.AttributeMgmt;
using FinalYearProject.Services.AuditTrails;
using FinalYearProject.Services.AuthMgmt;
using FinalYearProject.Services.Dashboard;
using FinalYearProject.Services.EmailTransactionsMgmt;
using FinalYearProject.Services.EmailTransactionsMgmt.Templates.Auth;
using FinalYearProject.Services.Encryption;
using FinalYearProject.Services.PolicyMgmt;
using FinalYearProject.Services.Shared.PolicyAuthorizationService;
using FinalYearProject.Services.Shared.UserContextService;
using FinalYearProject.Services.UploadsMgmt;
using FinalYearProject.Services.UserMgmt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace FinalYearProject.Data.Extensions;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// Register services to the DI
    /// <param name="services"></param>
    /// </summary>
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddHttpClient();

        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<IUserMgmtService, UserMgmtService>();
        services.AddScoped<IAttributeMgmtService, AttributeMgmtService>();
        services.AddScoped<IUploadsMgmtService, UploadsMgmtService>();
        services.AddScoped<IPolicyMgmtService, PolicyMgmtService>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddScoped<IEmailTransactionsMgmtService, EmailTransactionsMgmtService>();
        services.AddScoped<IAuthEmailTemplates, AuthEmailTemplates>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IAuditLogMgmtService, AuditLogMgmtService>();
        services.AddScoped<IPolicyAuthorizationService, PolicyAuthorizationService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMemoryCache, MemoryCache>();
    }

    /// <summary>
    /// Configure lazy loading of navigation property, Register SqlServer
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString"></param>
    public static void RegisterDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<FileSystemDbContext>(options =>
        {
            options.UseLazyLoadingProxies();
            options.UseNpgsql(connectionString, s =>
            {
                s.EnableRetryOnFailure(3);
            });
        });
    }

    /// <summary>
    /// Register authentications with jwt.
    /// </summary>
    /// <param name="services"></param>
    public static void RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DataProtectionTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromHours(24));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtKey = configuration["FileSystemConfig:JwtConfig:JwtKey"];
            var issuer = configuration["FileSystemConfig:JwtConfig:JwtIssuer"];
            var audience = configuration["FileSystemConfig:JwtConfig:JwtAudience"];

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey!)
                )
            };
        });

        services.AddAuthorization();
    }

    /// <summary>
    /// Binds the configuration file to the FileSystemConfig class which has the same section names and structure with the FileSystemConfig section in the configuration file.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static FileSystemConfig BindConfiguration(this IServiceCollection services, IConfiguration configuration)
    {


        FileSystemConfig appConfig = new();
        configuration.GetSection(nameof(FileSystemConfig)).Bind(appConfig);
        services.AddSingleton(appConfig);
        return appConfig;
    }
}
