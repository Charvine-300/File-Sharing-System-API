
using FinalYearProject.Data.Utilities;

namespace FinalYearProject.Services.UserMgmt;

public interface IUserMgmtService
{
    Task<ServiceResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task<ServiceResponse<UserResponse>> GetUserAsync(Guid id, CancellationToken cancellationToken);
    Task<ServiceResponse> DeleteUserAsync(Guid id, CancellationToken cancellationToken);
    Task<ServiceResponse> UpdateUserAsync(
    Guid id,
    UpdateUserRequest request,
    CancellationToken cancellationToken);

    Task<ServiceResponse> UpdateUserAttributesAsync(
        Guid id,
        UpdateUserAttributesRequest request,
        CancellationToken cancellationToken);

    Task<ServiceResponse<PaginationResponse<AllUsersResponse>>> GetUsersAsync(
    UserParameters parameters,
    CancellationToken cancellationToken);

    Task<ServiceResponse> UpdateUserStatusAsync(
        Guid id,
        UserStatusRequest request,
        CancellationToken cancellationToken);
}