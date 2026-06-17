
using FinalYearProject.Data.Utilities;

namespace FinalYearProject.Services.UserMgmt;

public interface IUserMgmtService
{
    Task<ServiceResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task<ServiceResponse<UserResponse>> GetUserAsync(Guid id, CancellationToken cancellationToken);
    Task<ServiceResponse> DeleteUserAsync(Guid id, CancellationToken cancellationToken);
}