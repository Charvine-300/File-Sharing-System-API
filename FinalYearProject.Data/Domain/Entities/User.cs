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
    public string PrivateKey { get; set; }
    public string? ProfilePhotoPublicId { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public virtual ICollection<Upload> Uploads { get; set; } = new List<Upload>();
    public virtual ICollection<UserAttribute> UsersAttributes { get; set; } = new List<UserAttribute>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
