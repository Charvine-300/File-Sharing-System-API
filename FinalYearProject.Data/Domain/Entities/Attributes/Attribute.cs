using FinalYearProject.Data.Domain.Entities;
using FinalYearProject.Data.Domain.Entities.Shared;

namespace FinalYearProject.Data.Domain.Entities.Attributes;

public class Attribute: BaseEntity
{
    public string AttributeName { get; set; }
    public AttributeType AttributeType { get; set; }
    public virtual ICollection<UserAttribute> UsersAttributes { get; set; } = new List<UserAttribute>();
}
