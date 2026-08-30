namespace FinalYearProject.Services.EmailTransactionsMgmt;

public interface IEmailTransactionsMgmtService
{
    Task<bool> SendEmailAsync(SendEmailRequest request, CancellationToken cancellationToken);
    Task<bool> SendBulkEmailAsync(BulkEmailRequest request, CancellationToken cancellationToken);
}
