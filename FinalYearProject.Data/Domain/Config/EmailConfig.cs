namespace FinalYearProject.Data.Domain.Config;

public class EmailConfig
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ProdURL { get; set; }
    public bool EnableSSL { get; set; }
}
