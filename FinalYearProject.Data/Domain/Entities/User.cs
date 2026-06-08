using FinalYearProject.Data.Domain.Entities.Attributes;
using FinalYearProject.Data.Domain.Entities.Shared;


namespace FinalYearProject.Data.Domain.Entities;

public class User: BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserType UserType { get; set; }
    public bool IsActive { get; set; }
    public virtual ICollection<Upload> Uploads { get; set; } = new List<Upload>();
    public virtual ICollection<UserAttribute> UsersAttributes { get; set; } = new List<UserAttribute>();

    // Confirm and add the column for user's generated private key
    // TODO: Add M-to-M realationship between User and Attributes
}
