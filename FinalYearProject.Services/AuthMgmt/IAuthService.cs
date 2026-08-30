using FinalYearProject.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinalYearProject.Services.AuthMgmt;

public interface IAuthService
{
    Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<ServiceResponse> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken);
    Task<ServiceResponse> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken);
    Task<ServiceResponse> VerifyOtpAsync(VerifyResetOtpRequest request, CancellationToken cancellationToken);
    Task<ServiceResponse> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken);
}
