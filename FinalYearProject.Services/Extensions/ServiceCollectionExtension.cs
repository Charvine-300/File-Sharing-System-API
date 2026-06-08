using FinalYearProject.Data.Context;
using FinalYearProject.Data.Domain.Config;
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

        services.AddScoped<IUserMgmtService, UserMgmtService>();
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
            options.UseSqlServer(connectionString, s =>
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
