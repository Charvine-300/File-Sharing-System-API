namespace FinalYearProject.Data.Domain.Config;

public record JwtConfig
{
    public string JwtKey { get; set; } = string.Empty;
    public string JwtIssuer { get; set; } = string.Empty;
    public string JwtAudience { get; set; } = string.Empty;
    public long JwtExpireMinutes { get; set; }
}
