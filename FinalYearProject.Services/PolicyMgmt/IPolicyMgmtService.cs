using FinalYearProject.Data.Utilities;


namespace FinalYearProject.Services.PolicyMgmt;

public interface IPolicyMgmtService
{
    /// <summary>
    /// Retrieve all policies
    /// </summary>
    Task<ServiceResponse<PaginationResponse<AllPoliciesResponse>>> GetPoliciesAsync(
        PolicyParameters parameters,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieve a single policy
    /// </summary>
    Task<ServiceResponse<PolicyDetailsResponse>> GetPolicyAsync(
        Guid id,
        CancellationToken cancellationToken);

    /// <summary>
    /// Create a new policy
    /// </summary>
    Task<ServiceResponse> CreatePolicyAsync(
        CreatePolicyRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Update an existing policy
    /// </summary>
    Task<ServiceResponse> UpdatePolicyAsync(
        Guid id,
        UpdatePolicyRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Delete a policy
    /// </summary>
    Task<ServiceResponse> DeletePolicyAsync(
        Guid id,
        CancellationToken cancellationToken);
}