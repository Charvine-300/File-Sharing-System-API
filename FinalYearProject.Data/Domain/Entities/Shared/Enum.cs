namespace FinalYearProject.Data.Domain.Entities.Shared;

public enum UserType
{
    Regular,
    SuperAdmin
}


public enum AttributeType
{
    Role,
    Department,
    ClearanceLevel,
    Other
}

public enum PolicyOperator
{
    And,
    Or
}