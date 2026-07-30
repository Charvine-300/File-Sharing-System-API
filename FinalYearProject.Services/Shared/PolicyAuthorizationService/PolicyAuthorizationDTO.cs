using FinalYearProject.Data.Domain.Entities.Shared;

namespace FinalYearProject.Services.Shared.PolicyAuthorizationService;

public class RuleNode
{
    // Leaf node
    public Guid? AttributeId { get; set; }

    // AND / OR
    public RuleOperator? Operator { get; set; }

    public RuleNode? Left { get; set; }

    public RuleNode? Right { get; set; }
}

public enum RuleOperator
{
    And,
    Or
}

public class PolicyAuthorizationResult
{
    public bool IsAuthorized { get; set; }

    public string? FailureReason { get; set; }

    public List<Guid> MissingAttributes { get; set; } = [];
}