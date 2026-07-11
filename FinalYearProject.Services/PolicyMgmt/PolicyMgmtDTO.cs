using FinalYearProject.Data.Domain.Entities.Shared;
using FinalYearProject.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinalYearProject.Services.PolicyMgmt;

public record AllPoliciesResponse(
    Guid Id,
    string PolicyName,
    string PolicyExpression,
    bool IsSystemPolicy
);

public record PolicyDetailsResponse(
    Guid Id,
    string PolicyName,
    string PolicyExpression,
    string Description,
    bool IsSystemPolicy
);

public class PolicyParameters : RequestParameters
{
}

public class CreatePolicyRequest
{
    public string PolicyName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public PolicyNodeRequest Rules { get; set; }
}
public class PolicyNodeRequest
{
    public Guid? AttributeId { get; set; }

    public PolicyOperator? Operator { get; set; }

    public List<PolicyNodeRequest> Children { get; set; } = new();
}

public class UpdatePolicyRequest
{
    public string PolicyName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
