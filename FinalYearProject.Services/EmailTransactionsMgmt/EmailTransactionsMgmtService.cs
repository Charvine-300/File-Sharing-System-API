using FinalYearProject.Data.Domain.Config;
using Serilog;
using System.Net;
using System.Net.Mail;

namespace FinalYearProject.Services.EmailTransactionsMgmt;

public class EmailTransactionsMgmtService(FileSystemConfig finalYearProjectConfig): IEmailTransactionsMgmtService
{
    public async Task<bool> SendEmailAsync(
     SendEmailRequest request,
     CancellationToken cancellationToken)
    {
        try
        {
            using SmtpClient client = new(
                finalYearProjectConfig.EmailConfig.Host,
                finalYearProjectConfig.EmailConfig.Port)
            {
                Credentials = new NetworkCredential(
                    finalYearProjectConfig.EmailConfig.Username,
                    finalYearProjectConfig.EmailConfig.Password),

                EnableSsl = finalYearProjectConfig.EmailConfig.EnableSSL,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            using MailMessage mailMessage = new()
            {
                From = new MailAddress(
                    finalYearProjectConfig.EmailConfig.Username,
                    "Zenly"),

                Subject = request.Subject,
                Body = request.Body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(request.To);

            await client.SendMailAsync(mailMessage, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");

            Log.Logger.Error(
                ex,
                "Error sending email to {Email}",
                request.To);

            return false;
        }
    }

    public async Task<bool> SendBulkEmailAsync(
    BulkEmailRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            using SmtpClient client = new(
                finalYearProjectConfig.EmailConfig.Host,
                finalYearProjectConfig.EmailConfig.Port)
            {
                Credentials = new NetworkCredential(
                    finalYearProjectConfig.EmailConfig.Username,
                    finalYearProjectConfig.EmailConfig.Password),

                EnableSsl = finalYearProjectConfig.EmailConfig.EnableSSL,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            using MailMessage mailMessage = new()
            {
                From = new MailAddress(
                    finalYearProjectConfig.EmailConfig.Username,
                    "Zenly"),

                Subject = request.Subject,
                Body = request.Body,
                IsBodyHtml = true
            };

            foreach (string email in request.To.Distinct())
            {
                mailMessage.Bcc.Add(email);
            }

            await client.SendMailAsync(mailMessage, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Error sending bulk email");

            return false;
        }
    }
}
