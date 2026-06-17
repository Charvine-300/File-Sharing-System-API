using FinalYearProject.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinalYearProject.Services.AttributeMgmt;

public interface IAttributeMgmtService
{
    Task<ServiceResponse<PaginationResponse<AllAttributesResponse>>> GetAttributesAsync(AttributeParameters parameters, CancellationToken cancellationToken);

    Task<ServiceResponse<AttributeDetailsResponse>> GetAttributeDetailsAsync(Guid id, CancellationToken cancellationToken);

    Task<ServiceResponse> CreateAttributeAsync(AttributeMgmtRequest request, CancellationToken cancellationToken);

    Task<ServiceResponse> UpdateAttributeAsync(Guid id, AttributeMgmtRequest request, CancellationToken cancellationToken);

    Task<ServiceResponse> DeleteAttributeAsync(Guid id, CancellationToken cancellationToken);
}
