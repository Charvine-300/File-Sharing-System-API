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

public enum ActionType
{
    Unknown = 0,
    Create = 1,
    Update = 2,
    Delete = 3,
    Authentication = 4,
    Other = 5
}