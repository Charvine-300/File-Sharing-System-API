using FinalYearProject.Data.Context;
using FinalYearProject.Data.Domain.Entities.Shared;

namespace FinalYearProject.Services.Shared.PolicyAuthorizationService;

public class PolicyAuthorizationService(FileSystemDbContext database) : IPolicyAuthorizationService
{
    public PolicyAuthorizationResult CanCreatePolicy(
        PolicyNodeRequest rules,
        IEnumerable<Guid> userAttributeIds,
        bool isSuperAdmin)
    {
        if (isSuperAdmin)
        {
            return new PolicyAuthorizationResult
            {
                IsAuthorized = true
            };
        }

        return Evaluate(
            rules,
            userAttributeIds.ToHashSet());
    }

    public PolicyAuthorizationResult CanBypassPolicy(
       string policyExpression,
       IEnumerable<Guid> userAttributeIds,
       bool isSuperAdmin)
    {
        if (isSuperAdmin)
        {
            return new PolicyAuthorizationResult
            {
                IsAuthorized = true
            };
        }

        var attributeLookup = database.Attributes
            .ToDictionary(
                a => a.AttributeName,
                a => a.Id);

        var tree = ParsePolicyExpression(
            policyExpression,
            attributeLookup);

        return Evaluate(
            tree,
            userAttributeIds.ToHashSet());
    }

    private PolicyAuthorizationResult Evaluate(
        PolicyNodeRequest node,
        HashSet<Guid> userAttributes)
    {
        // Leaf node
        if (node.AttributeId.HasValue)
        {
            if (userAttributes.Contains(node.AttributeId.Value))
            {
                return new PolicyAuthorizationResult
                {
                    IsAuthorized = true
                };
            }

            return new PolicyAuthorizationResult
            {
                IsAuthorized = false,
                FailureReason = "Required attribute missing.",
                MissingAttributes = new()
                {
                    node.AttributeId.Value
                }
            };
        }

        if (!node.Operator.HasValue)
        {
            throw new InvalidOperationException(
                "Operator node must contain an operator.");
        }

        if (node.Children == null || node.Children.Count == 0)
        {
            throw new InvalidOperationException(
                "Operator node must contain at least one child.");
        }

        switch (node.Operator.Value)
        {
            case PolicyOperator.And:

                var andMissing = new List<Guid>();

                foreach (var child in node.Children)
                {
                    var result = Evaluate(
                        child,
                        userAttributes);

                    if (!result.IsAuthorized)
                    {
                        andMissing.AddRange(
                            result.MissingAttributes);
                    }
                }

                if (!andMissing.Any())
                {
                    return new PolicyAuthorizationResult
                    {
                        IsAuthorized = true
                    };
                }

                return new PolicyAuthorizationResult
                {
                    IsAuthorized = false,
                    FailureReason = "One or more required attributes are missing.",
                    MissingAttributes = andMissing
                        .Distinct()
                        .ToList()
                };

            case PolicyOperator.Or:

                PolicyAuthorizationResult? bestFailure = null;

                foreach (var child in node.Children)
                {
                    var result = Evaluate(
                        child,
                        userAttributes);

                    if (result.IsAuthorized)
                    {
                        return result;
                    }

                    if (bestFailure == null ||
                        result.MissingAttributes.Count <
                        bestFailure.MissingAttributes.Count)
                    {
                        bestFailure = result;
                    }
                }

                return new PolicyAuthorizationResult
                {
                    IsAuthorized = false,
                    FailureReason = "You do not satisfy any branch of this policy.",
                    MissingAttributes = bestFailure?.MissingAttributes
                        ?? new List<Guid>()
                };

            default:
                throw new InvalidOperationException(
                    "Unsupported policy operator.");
        }
    }

    private PolicyNodeRequest ParsePolicyExpression(
    string expression,
    Dictionary<string, Guid> attributeLookup)
    {
        expression = expression.Trim();

        // Remove outer brackets if they wrap the whole expression
        while (expression.StartsWith("(") &&
               expression.EndsWith(")") &&
               IsOuterParentheses(expression))
        {
            expression = expression[1..^1].Trim();
        }

        int depth = 0;

        for (int i = 0; i < expression.Length; i++)
        {
            switch (expression[i])
            {
                case '(':
                    depth++;
                    break;

                case ')':
                    depth--;
                    break;
            }

            if (depth != 0)
                continue;

            // Look for OR first (lowest precedence)
            if (StartsWithOperator(expression, i, " or "))
            {
                string left = expression[..i];

                string right = expression[(i + 4)..];

                return new PolicyNodeRequest
                {
                    Operator = PolicyOperator.Or,
                    Children =
                    [
                        ParsePolicyExpression(left, attributeLookup),
                        ParsePolicyExpression(right, attributeLookup)
                    ]
                };
            }
        }

        depth = 0;

        for (int i = 0; i < expression.Length; i++)
        {
            switch (expression[i])
            {
                case '(':
                    depth++;
                    break;

                case ')':
                    depth--;
                    break;
            }

            if (depth != 0)
                continue;

            // Look for AND
            if (StartsWithOperator(expression, i, " and "))
            {
                string left = expression[..i];

                string right = expression[(i + 5)..];

                return new PolicyNodeRequest
                {
                    Operator = PolicyOperator.And,
                    Children =
                    [
                        ParsePolicyExpression(left, attributeLookup),
                        ParsePolicyExpression(right, attributeLookup)
                    ]
                };
            }
        }

        // Leaf node
        expression = expression.Trim();

        if (!attributeLookup.TryGetValue(expression, out Guid attributeId))
        {
            throw new InvalidOperationException(
                $"Unknown attribute '{expression}'.");
        }

        return new PolicyNodeRequest
        {
            AttributeId = attributeId
        };
    }

    private static bool StartsWithOperator(
    string expression,
    int index,
    string op)
    {
        if (index + op.Length > expression.Length)
        {
            return false;
        }

        return expression.Substring(index, op.Length) == op;
    }

    private static bool IsOuterParentheses(
    string expression)
    {
        int depth = 0;

        for (int i = 0; i < expression.Length; i++)
        {
            switch (expression[i])
            {
                case '(':
                    depth++;
                    break;

                case ')':
                    depth--;

                    if (depth == 0 && i != expression.Length - 1)
                    {
                        return false;
                    }

                    break;
            }
        }

        return true;
    }
}