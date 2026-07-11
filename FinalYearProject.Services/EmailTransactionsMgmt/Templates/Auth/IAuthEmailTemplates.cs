namespace FinalYearProject.Services.EmailTransactionsMgmt.Templates.Auth;

public interface IAuthEmailTemplates
{
    string AccountCreationTemplate(
        string firstName,
        string email,
        string password,
        string loginUrl);
}