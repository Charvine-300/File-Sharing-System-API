using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FinalYearProject.Data.Extensions;

public static class SwaggerExtension
{
    /// <summary>
    /// Add bearer token and configures swagger documentation generation.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblyName"></param>
    /// <param name="environmentName"></param>
    public static void ConfigureSwagger(this IServiceCollection services, string assemblyName, string environmentName)
    {
        services.AddSwaggerGen(option =>
        {
            string xmlfile = $"{assemblyName}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlfile);
            option.IncludeXmlComments(xmlPath);

            option.SwaggerDoc("v1", new OpenApiInfo { Title = $"File Sharing System - {environmentName}", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type=ReferenceType.SecurityScheme,
                Id="Bearer"
            }
        },
        new string[]{}
    }
    });
        });
    }

}
