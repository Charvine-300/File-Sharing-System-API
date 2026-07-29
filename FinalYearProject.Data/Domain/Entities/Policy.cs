using FinalYearProject.Data.Domain.Entities.Attributes;
using FinalYearProject.Data.Domain.Entities.Shared;

namespace FinalYearProject.Data.Domain.Entities;

public class Policy: BaseEntity
{
    public string PolicyName { get; set; }
    public string PolicyExpression { get; set; }
    public string Description { get; set; }
    public bool IsSystemPolicy { get; set; }
    public virtual ICollection<Upload> Uploads { get; set; } = new List<Upload>();
    public virtual ICollection<PolicyAttribute> PoliciesAttributes { get; set; } = new List<PolicyAttribute>();
}
