namespace FinalYearProject.Services.EmailTransactionsMgmt.Templates.Auth;

public class AuthEmailTemplates: IAuthEmailTemplates
{
    public string AccountCreationTemplate(
        string firstName,
        string email,
        string password,
        string loginUrl)
    {
        return $@"
<div style='font-family: Arial, sans-serif; background-color:#f5f7fb; padding:40px;'>


    <div style='max-width:600px;
                margin:0 auto;
                background-color:white;
                border-radius:12px;
                overflow:hidden;
                box-shadow:0 4px 12px rgba(0,0,0,0.08);'>

        <div style='background-color:#2563eb;
                    padding:24px;
                    text-align:center;'>

            <h1 style='color:white; margin:0;'>
                Welcome to Vault 🎉
            </h1>
        </div>

        <div style='padding:32px;'>

            <h2 style='margin-top:0; color:#111827;'>
                Hello {firstName},
            </h2>

            <p style='font-size:16px; color:#4b5563;'>
                Your account has been successfully created.
                You can now log in and access the Vault File Sharing System.
            </p>

            <div style='background:#f3f4f6;
                        padding:20px;
                        border-radius:8px;
                        margin:24px 0;'>

                <h3 style='margin-top:0;'>
                    Login Credentials
                </h3>

                <p>
                    <strong>Email:</strong> {email}
                </p>

                <p>
                    <strong>Password:</strong> {password}
                </p>
            </div>

            <div style='text-align:center; margin:32px 0;'>

                <a href='{loginUrl}'
                   style='background-color:#2563eb;
                          color:white;
                          text-decoration:none;
                          padding:14px 28px;
                          border-radius:8px;
                          display:inline-block;
                          font-weight:bold;'>

                    Login to Zenly

                </a>

            </div>

            <p style='color:#6b7280;'>
                For security reasons, please change your password after your first login.
            </p>

            <p style='margin-top:32px;'>

                Regards,<br/>
                <strong>Vault Team</strong>

            </p>

        </div>

    </div>

</div>";
    }

    public string PasswordResetOtpTemplate(
    string firstName,
    string otp, int time)
    {
        return $@"
<div style='font-family:Arial,sans-serif;background:#f5f7fb;padding:40px;'>

<div style='max-width:600px;margin:auto;background:white;border-radius:12px;padding:32px;'>

<h2>Hello {firstName},</h2>

<p>
We received a request to reset your password.
</p>

<p>
Use the OTP below to continue.
</p>

<div style='font-size:36px;
            font-weight:bold;
            text-align:center;
            letter-spacing:8px;
            color:#2563eb;
            margin:30px 0;'>

{otp}

</div>

<p>
This code expires in
<strong>{time} minutes</strong>.
</p>

<p>
If you did not request this,
please ignore this email.
</p>

<p>

Regards,<br/>

<strong>Vault Team</strong>

</p>

</div>

</div>";
    }
}