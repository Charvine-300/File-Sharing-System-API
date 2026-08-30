namespace FinalYearProject.Services.EmailTransactionsMgmt;

public class SendEmailRequest
{
    public string To { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
}

public class BulkEmailRequest
{
    public List<string> To { get; set; } = [];
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
