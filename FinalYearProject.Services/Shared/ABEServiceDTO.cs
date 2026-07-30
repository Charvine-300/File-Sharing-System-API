using FinalYearProject.Data.Domain.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FinalYearProject.Services.Shared;

public class PythonKeyResponse
{
    [JsonPropertyName("private_key")]
    public string PrivateKey { get; set; }
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