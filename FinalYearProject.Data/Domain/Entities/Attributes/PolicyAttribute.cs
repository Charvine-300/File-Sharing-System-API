using FinalYearProject.Data.Domain.Entities.Shared;

namespace FinalYearProject.Data.Domain.Entities.Attributes;

public class PolicyAttribute: BaseEntity
{
    public Guid AttributeId { get; set; }
    public virtual Attribute Attribute { get; set; }
    public Guid PolicyId { get; set; }
    public virtual Policy Policy { get; set; }
}
