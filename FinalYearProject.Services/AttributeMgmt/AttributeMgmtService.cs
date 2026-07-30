using FinalYearProject.Data.Context;
using FinalYearProject.Data.Domain.Entities.Shared;
using FinalYearProject.Data.Utilities;
using FinalYearProject.Services.AuditTrails;
using FinalYearProject.Services.Shared;
using FinalYearProject.Services.Shared.UserContextService;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Attribute = FinalYearProject.Data.Domain.Entities.Attributes.Attribute;


namespace FinalYearProject.Services.AttributeMgmt;

public class AttributeMgmtService(
    FileSystemDbContext database,
    IUserContextService userContextService,
    IAuditLogMgmtService auditLogService
) : IAttributeMgmtService
{
    public async Task<ServiceResponse<PaginationResponse<AllAttributesResponse>>> GetAttributesAsync(
        AttributeParameters parameters,
        CancellationToken cancellationToken)
    {
        IQueryable<Attribute> query =  database.Attributes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            query = query.Where(a => a.AttributeName.ToLower().Contains(parameters.Search.ToLower()));
        }

        if (parameters.AttributeType.HasValue)
        {
            query = query.Where(a => a.AttributeType == parameters.AttributeType);
        }

        // Removing the Super Admin option
        query = query.Where(a => a.AttributeName.ToLower() != "superadmin");

        int totalCount = await query.CountAsync(cancellationToken);
        int totalPages = (int)Math.Ceiling((double)totalCount / parameters.PageSize);

        var attributes = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        var data = attributes.Select(a => new AllAttributesResponse(
            a.Id,
            a.AttributeName,
            a.AttributeType.ToString()
        )).ToList();

        return Response.Success(
            "Attributes retrieved successfully",
            new PaginationResponse<AllAttributesResponse>(
                Records: data,
                TotalRecords: totalCount,
                TotalPages: totalPages,
                CurrentPage: parameters.PageNumber,
                PageSize: parameters.PageSize
            )
        );
    }

    public async Task<ServiceResponse<AttributeDetailsResponse>> GetAttributeDetailsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var attribute = await database.Attributes
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (attribute == null)
            return Response.NotFound<AttributeDetailsResponse>("Attribute not found", null!);

        var data = new AttributeDetailsResponse(
            attribute.Id,
            attribute.AttributeName,
            attribute.AttributeType
        );

        return Response.Success("Attribute details retrieved successfully", data);
    }

    public async Task<ServiceResponse> CreateAttributeAsync(
        AttributeMgmtRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            bool exists = await database.Attributes.AnyAsync(
                a => a.AttributeName.ToLower() == request.AttributeName.ToLower()
                     && a.AttributeType == request.AttributeType,
                cancellationToken);

            if (exists)
                return Response.Conflict("Attribute already exists");

            var attribute = new Attribute
            {
                AttributeName = request.AttributeName,
                AttributeType = request.AttributeType,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await auditLogService.CreateAuditLogAsync(
                new CreateAuditTrailRequest
                {
                    Action = "Create Attribute",
                    Description = $"Created attribute {attribute.AttributeName} of type {attribute.AttributeType}",
                    Actor = $"{userContextService.User.FirstName} {userContextService.User.LastName}",
                    ActionType = ActionType.Create
                },
                cancellationToken
            );

            database.Attributes.Add(attribute);
            await database.SaveChangesAsync(cancellationToken);

            return Response.Created("Attribute created successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating attribute");
            return Response.SystemMalfunction("Something went wrong");
        }
    }

    public async Task<ServiceResponse> UpdateAttributeAsync(
       Guid id,
       AttributeMgmtRequest request,
       CancellationToken cancellationToken)
    {
        try
        {
            var attribute = await database.Attributes
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (attribute == null)
                return Response.NotFound("Attribute not found");

            bool isAssigned = await database.Users_Attributes
                .AnyAsync(ua => ua.AttributeId == id, cancellationToken);

            if (isAssigned)
            {
                return Response.Conflict(
                    "This attribute has already been assigned to one or more users and cannot be updated.");
            }

            // Prevent duplicate attributes
            bool exists = await database.Attributes.AnyAsync(
                a => a.Id != id &&
                     a.AttributeName.ToLower() == request.AttributeName.ToLower() &&
                     a.AttributeType == request.AttributeType,
                cancellationToken);

            if (exists)
            {
                return Response.Conflict(
                    "An attribute with the same name and type already exists.");
            }

            attribute.AttributeName = request.AttributeName;
            attribute.AttributeType = request.AttributeType;
            attribute.ModifiedAt = DateTimeOffset.UtcNow;

            await auditLogService.CreateAuditLogAsync(
                 new CreateAuditTrailRequest
                 {
                     Action = "Update Attribute",
                     Description = $"Updated attribute {attribute.AttributeName} of type {attribute.AttributeType}",
                     Actor = $"{userContextService.User.FirstName} {userContextService.User.LastName}",
                     ActionType = ActionType.Update
                 },
                 cancellationToken
             );

            await database.SaveChangesAsync(cancellationToken);

            return Response.Success("Attribute updated successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating attribute");
            return Response.SystemMalfunction("Something went wrong");
        }
    }

    public async Task<ServiceResponse> DeleteAttributeAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var attribute = await database.Attributes
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (attribute == null)
                return Response.NotFound("Attribute not found");

            bool isAssigned = await database.Users_Attributes
               .AnyAsync(ua => ua.AttributeId == id, cancellationToken);

            if (isAssigned)
            {
                return Response.Conflict(
                    "This attribute has already been assigned to one or more users and cannot be deleted.");
            }


            await auditLogService.CreateAuditLogAsync(
                 new CreateAuditTrailRequest
                 {
                     Action = "Delete Attribute",
                     Description = $"Deleted attribute {attribute.AttributeName} of type {attribute.AttributeType}",
                     Actor = $"{userContextService.User.FirstName} {userContextService.User.LastName}",
                     ActionType = ActionType.Delete
                 },
                 cancellationToken
             );

            database.Attributes.Remove(attribute);
            await database.SaveChangesAsync(cancellationToken);

            return Response.Success("Attribute deleted successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting attribute");
            return Response.SystemMalfunction("Something went wrong");
        }
    }
}
