using Serilog;
using Elastic.Apm.SerilogEnricher;
using Serilog.Events;
using Serilog.Exceptions;
using FinalYearProject.Data.Domain.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace FinalYearProject.Data.Extensions;

public static class SerilogExtension
{

    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder, SerilogConfig serilogConfig)
    {

        IWebHostEnvironment environment = builder.Environment;
        string mainIndexFormat = $"{builder.Environment.ApplicationName}-{environment.EnvironmentName}".ToLower();

        string diagnosticIndexFormat = $"{builder.Environment.ApplicationName}-{environment.EnvironmentName}".ToLower();
        builder.Host.UseSerilog((context, config) =>
        {
            string logFileName = $"{DateTime.UtcNow:yyyy-MM-dd}-.txt";
            string logFilePath = $"logs/{environment.EnvironmentName.ToLower()}/{logFileName}";

            config
                  .WriteTo.Console()
                  .WriteTo.Async(writeTo => writeTo.File(
                  path: logFilePath,
                  rollingInterval: RollingInterval.Day,
                  shared: true,
                  outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}",
                  fileSizeLimitBytes: null))
                  .WriteTo.Logger(lc => lc
                  .Filter.ByExcluding(IsDiagnosticLog)
                  //.WriteTo.Async(writeTo => writeTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(serilogConfig.NodeURI))
                  //{
                  //    IndexFormat = mainIndexFormat,
                  //    AutoRegisterTemplate = true,
                  //    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                  //    BatchAction = ElasticOpType.Create,
                  //    TypeName = null,
                  //    NumberOfShards = 2,
                  //    NumberOfReplicas = 1,
                  //    CustomFormatter = new EcsTextFormatter(),
                  //    EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog,
                  //    ModifyConnectionSettings = x => x.ApiKeyAuthentication(new ApiKeyAuthenticationCredentials(serilogConfig.APIKey)),
                  //}))
                  .WriteTo.Logger(lc => lc
                  .Filter.ByIncludingOnly(IsDiagnosticLog)
                  //.WriteTo.Async(writeTo => writeTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(serilogConfig.NodeURI))
                  //{
                  //    IndexFormat = diagnosticIndexFormat,
                  //    AutoRegisterTemplate = true,
                  //    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                  //    BatchAction = ElasticOpType.Create,
                  //    TypeName = null,
                  //    NumberOfShards = 2,
                  //    NumberOfReplicas = 1,
                  //    CustomFormatter = new EcsTextFormatter(),
                  //    EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog,
                  //    ModifyConnectionSettings = x => x.ApiKeyAuthentication(new ApiKeyAuthenticationCredentials(serilogConfig.APIKey))
                  //}))
              .Enrich.FromLogContext()
              .Enrich.WithExceptionDetails()
              .Enrich.WithElasticApmCorrelationInfo()
              .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
              .ReadFrom.Configuration(context.Configuration)
              .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)));
        });
        return builder;
    }

    private static bool IsDiagnosticLog(LogEvent logEvent)
    {
        //Check SourceContext for diagnostic-related types
        if (logEvent.Properties.TryGetValue("SourceContext", out var sourceContext))
        {
            string sourceContextStr = sourceContext.ToString();
            if (sourceContextStr.Contains("Diagnostics", StringComparison.OrdinalIgnoreCase) ||
                sourceContextStr.Contains("Lifetime", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
