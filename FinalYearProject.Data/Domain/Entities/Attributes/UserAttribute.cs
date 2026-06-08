
using FinalYearProject.Data.Domain.Entities.Shared;

namespace FinalYearProject.Data.Domain.Entities.Attributes;

public class UserAttribute: BaseEntity
{
    public Guid AttributeId { get; set; }
    public virtual Attribute Attribute { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
}
