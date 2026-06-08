using FinalYearProject.Data.Domain.Config;
using FinalYearProject.Data.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Services
FileSystemConfig fileSystemConfig = builder.Services.BindConfiguration(builder.Configuration);
builder.ConfigureSerilog(fileSystemConfig.SerilogConfig);

builder.Services.RegisterDbContext(fileSystemConfig.ConnectionString);
builder.Services.RegisterServices();

builder.Services.RegisterAuthentication(builder.Configuration);
builder.Services.AddControllers(x =>
{
    x.EnableEndpointRouting = false;
}).AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddControllers();

// Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger UI (IMPORTANT)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();