namespace FinalYearProject.Data.Domain.Config;

/// <summary>
/// Serilog configuration
/// </summary>
public record SerilogConfig
{
    public string NodeURI { get; set; } = default!;
    public string APIKey { get; set; } = default!;
}
