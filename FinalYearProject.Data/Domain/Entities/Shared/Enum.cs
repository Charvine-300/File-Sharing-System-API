namespace FinalYearProject.Data.Domain.Entities.Shared;

public enum UserType
{
    Regular,
    SuperAdmin
}

public enum ComplaintType
{
    AssessmentsAndExams = 1,
    Projects = 2, 
    Presentations = 3,
    Other = 4
}

public enum ActionType
{
    Unknown = 0,
    Create = 1,
    Update = 2,
    Delete = 3,
    Other = 4
}

public enum UserStatus
{
    Active,
    Inactive,
    Suspended,
    Deleted
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