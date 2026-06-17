namespace FinalYearProject.Data.Domain.Config;

public class FileSystemConfig
{
    public string ConnectionString { get; set; } = default!;
    public bool IsProduction { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
    public string AESKey { get; set; } = default!;
    public string ABEBaseURL { get; set; }
    public SerilogConfig SerilogConfig { get; set; } = default!;
    public JwtConfig JwtConfig { get; set; } = default!;
    public CloudinaryConfig CloudinaryConfig { get; set; } = default!;
    public EmailConfig EmailConfig { get; set; } = default!;
}
