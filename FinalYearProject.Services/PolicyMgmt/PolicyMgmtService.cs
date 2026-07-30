using FinalYearProject.Data.Context;
using FinalYearProject.Data.Domain.Config;
using FinalYearProject.Data.Domain.Entities;
using FinalYearProject.Data.Domain.Entities.Attributes;
using FinalYearProject.Data.Domain.Entities.Shared;
using FinalYearProject.Data.Utilities;
using FinalYearProject.Services.AuditTrails;
using FinalYearProject.Services.Shared;
using FinalYearProject.Services.Shared.PolicyAuthorizationService;
using FinalYearProject.Services.Shared.UserContextService;
using FinalYearProject.Services.UploadsMgmt;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Attribute = FinalYearProject.Data.Domain.Entities.Attributes.Attribute;

namespace FinalYearProject.Services.PolicyMgmt;

public class PolicyMgmtService(FileSystemDbContext database, FileSystemConfig config, IAuditLogMgmtService auditLogService, IUserContextService userContextService, IPolicyAuthorizationService policyAuthorizationService) : IPolicyMgmtService
{
    public async Task<ServiceResponse<PaginationResponse<AllPoliciesResponse>>> GetPoliciesAsync(
      PolicyParameters parameters,
      CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<Policy> query =
                database.Policies
                .AsNoTracking();


            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                string search = parameters.Search.Trim().ToLower();

                query = query.Where(p =>
                    p.PolicyName.ToLower().Contains(search));
            }


            if (parameters.StartDate.HasValue)
            {
                query = query.Where(p =>
                    p.CreatedAt >= parameters.StartDate.Value);
            }


            if (parameters.EndDate.HasValue)
            {
                query = query.Where(p =>
                    p.CreatedAt <= parameters.EndDate.Value);
            }


            int totalRecords =
                await query.CountAsync(cancellationToken);


            int totalPages =
                (int)Math.Ceiling(
                    (double)totalRecords /
                    parameters.PageSize);


            List<Policy> policies =
                await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);



            List<AllPoliciesResponse> data =
                policies
                .Select(p => new AllPoliciesResponse(
                    p.Id,
                    p.PolicyName,
                    p.PolicyExpression,
                    p.IsSystemPolicy
                ))
                .ToList();



            return Response.Success(
                "Policies retrieved successfully",
                new PaginationResponse<AllPoliciesResponse>
                (
                    Records: data,
                    PageSize: parameters.PageSize,
                    CurrentPage: parameters.PageNumber,
                    TotalPages: totalPages,
                    TotalRecords: totalRecords
                ));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving policies");

            return Response.SystemMalfunction<
                PaginationResponse<AllPoliciesResponse>>
                ("An error occurred while retrieving policies", null!);
        }
    }



    public async Task<ServiceResponse<PolicyDetailsResponse>> GetPolicyAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            Policy? policy =
                await database.Policies
                .Include(f => f.Uploads)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.Id == id,
                    cancellationToken);


            if (policy == null)
            {
                return Response.NotFound<PolicyDetailsResponse>(
                    "Policy not found",
                    null!);
            }


            PolicyDetailsResponse response =
                new(
                    policy.Id,
                    policy.PolicyName,
                    policy.PolicyExpression,
                    policy.Description,
                    policy.IsSystemPolicy,
                    policy.Uploads.Select(u => new FileResponse(
                        u.Id,
                        u.Filename,
                        u.ContentType,
                        u.CreatedAt,
                        u.CreatedBy ?? "Unknown",
                        u.UserId
                    )).ToList()
                );


            return Response.Success(
                "Policy retrieved successfully",
                response);
        }
        catch (Exception ex)
        {
            Log.Error(ex,
                "Error retrieving policy {PolicyId}",
                id);


            return Response.SystemMalfunction<
                PolicyDetailsResponse>
                ("An error occurred while retrieving policy", null!);
        }
    }


    public async Task<ServiceResponse> CreatePolicyAsync(
        CreatePolicyRequest request,
        CancellationToken cancellationToken)
    {
        try
        {

            if (request.Rules == null)
            {
                return Response.BadRequest(
                    "At least one attribute is required");
            }



            // Validate attributes
            List<Guid> attributeIds = new();

            ExtractAttributeIds(
                request.Rules,
                attributeIds);



            attributeIds = attributeIds
                .Distinct()
                .ToList();

            List<Attribute> attributes = await database.Attributes
                .Where(a => attributeIds.Contains(a.Id))
                .ToListAsync(cancellationToken);

            if (attributes.Count != attributeIds.Count)
            {
                return Response.BadRequest(
                    "One or more attributes do not exist");
            }



            string expression =
                BuildPolicyExpression(
                    request.Rules,
                    attributes);



            bool exists =
                await database.Policies
                .AnyAsync(
                    p => p.PolicyExpression == expression,
                    cancellationToken);



            if (exists)
            {
                return Response.Conflict(
                    "A policy with this expression already exists");
            }

            var isSuperAdmin = userContextService.User.UserType == UserType.SuperAdmin;
            Console.WriteLine($"isSuperAdmin: {isSuperAdmin}");

            var currentUser = await database.Users
                .Include(u => u.UsersAttributes)
                .FirstOrDefaultAsync(u => u.Id == userContextService.User.Id, cancellationToken);

            Console.WriteLine(
                $"Current user attributes: {string.Join(", ", currentUser.UsersAttributes.Select(x => x.AttributeId))}");

            Console.WriteLine(
                $"Policy attributes: {string.Join(", ", attributeIds)}");

            Console.WriteLine(
                $"User attributes: {string.Join(", ", currentUser.UsersAttributes.Select(x => x.AttributeId))}");

            var authorizationResult =
                policyAuthorizationService.CanCreatePolicy(
                    request.Rules,
                    currentUser.UsersAttributes
                        .Select(x => x.AttributeId),
                    isSuperAdmin);

            if (!authorizationResult.IsAuthorized)
            {
                return Response.Forbidden(
                    authorizationResult.FailureReason);
            }

            Policy policy = new()
            {
                PolicyName = request.PolicyName,

                Description = request.Description,

                PolicyExpression = expression,

                IsSystemPolicy = false,

                CreatedAt = DateTimeOffset.UtcNow,

            };

            database.Policies.Add(policy);

            var policyAttributes = attributeIds
                 .Distinct()
                 .Select(attributeId => new PolicyAttribute
                 {
                     PolicyId = policy.Id,
                     AttributeId = attributeId,
                     CreatedAt = DateTimeOffset.UtcNow
                 });

            database.Policies_Attributes.AddRange(policyAttributes);

            await auditLogService.CreateAuditLogAsync(
                  new CreateAuditTrailRequest
                  {
                      Action = "Create Policy",
                      Description = $"{policy.PolicyName} policy created successfully",
                      Actor = $"{userContextService.User.FirstName} {userContextService.User.LastName}",
                      ActionType = ActionType.Create
                  },
                  cancellationToken
              );

            await database.SaveChangesAsync(
                cancellationToken);
            return Response.Created(
                "Policy created successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex,
                "Error creating policy");


            return Response.SystemMalfunction(
                "An error occurred while creating policy");
        }
    }



    private string BuildPolicyExpression(
     PolicyNodeRequest node,
     List<Attribute> attributes)
    {
        // Leaf node
        if (node.AttributeId.HasValue)
        {
            var attribute =
                attributes.FirstOrDefault(
                    a => a.Id == node.AttributeId.Value);


            if (attribute == null)
            {
                throw new Exception(
                    "Attribute does not exist");
            }


            return attribute.AttributeName;
        }

        // Operator node
        if (!node.Operator.HasValue ||
            node.Children == null ||
            node.Children.Count < 2)
        {
            throw new Exception(
                "Invalid policy structure");
        }



        var childExpressions =
            node.Children
                .Select(
                    child =>
                        BuildPolicyExpression(
                            child,
                            attributes))
                .ToList();



        string joined =
            string.Join(
                $" {node.Operator.Value.ToString().ToLower()} ",
                childExpressions);



        return $"({joined})";
    }

    public async Task<ServiceResponse> UpdatePolicyAsync(
    Guid id,
    UpdatePolicyRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            Policy? policy =
                await database.Policies
                    .FirstOrDefaultAsync(
                        p => p.Id == id,
                        cancellationToken);


            if (policy == null)
            {
                return Response.NotFound(
                    "Policy not found");
            }


            // Prevent editing system policies
            if (policy.IsSystemPolicy)
            {
                return Response.BadRequest(
                    "System policies cannot be modified");
            }


            policy.PolicyName =
                request.PolicyName;


            policy.Description =
                request.Description;


            policy.ModifiedAt =
                DateTimeOffset.UtcNow;

            await auditLogService.CreateAuditLogAsync(
                new CreateAuditTrailRequest
                {
                    Action = "Update Policy",
                    Description = $"{policy.PolicyName} policy updated successfully",
                    Actor = $"{userContextService.User.FirstName} {userContextService.User.LastName}",
                    ActionType = ActionType.Update
                },
                cancellationToken
            );

            await database.SaveChangesAsync(
                cancellationToken);
            return Response.Success(
                "Policy updated successfully");
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "Error updating policy {PolicyId}",
                id);


            return Response.SystemMalfunction(
                "An error occurred while updating policy");
        }
    }



    public async Task<ServiceResponse> DeletePolicyAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            Policy? policy =
                await database.Policies
                .FirstOrDefaultAsync(
                    p => p.Id == id,
                    cancellationToken);



            if (policy == null)
            {
                return Response.NotFound(
                    "Policy not found");
            }



            // System policies should never be removed
            if (policy.IsSystemPolicy)
            {
                return Response.BadRequest(
                    "System policies cannot be deleted");
            }



            // Check if policy is currently attached to files
            bool hasUploads =
                await database.Uploads
                .AnyAsync(
                    u => u.PolicyId == id,
                    cancellationToken);



            if (hasUploads)
            {
                return Response.BadRequest(
                    "This policy cannot be deleted because it is attached to uploaded files");
            }

            // Remove all policy-attribute mappings first
            var policyAttributes = await database.Policies_Attributes
                .Where(pa => pa.PolicyId == id)
                .ToListAsync(cancellationToken);

            database.Policies_Attributes.RemoveRange(policyAttributes);
            database.Policies.Remove(policy);

            await auditLogService.CreateAuditLogAsync(
                new CreateAuditTrailRequest
                {
                    Action = "Delete Policy",
                    Description = $"{policy.PolicyName} policy deleted successfully",
                    Actor = $"{userContextService.User.FirstName} {userContextService.User.LastName}",
                    ActionType = ActionType.Delete
                },
                cancellationToken
            );

            await database.SaveChangesAsync(
                cancellationToken);

            return Response.Success(
                "Policy deleted successfully");
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "Error deleting policy {PolicyId}",
                id);


            return Response.SystemMalfunction(
                "An error occurred while deleting policy");
        }
    }

    private void ExtractAttributeIds(
    PolicyNodeRequest node,
    List<Guid> ids)
    {
        if (node.AttributeId.HasValue)
        {
            ids.Add(node.AttributeId.Value);
            return;
        }


        if (node.Children != null)
        {
            foreach (var child in node.Children)
            {
                ExtractAttributeIds(child, ids);
            }
        }
    }
}
