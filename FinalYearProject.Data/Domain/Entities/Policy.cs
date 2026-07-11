using FinalYearProject.Data.Domain.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinalYearProject.Data.Domain.Entities;

public class Policy: BaseEntity
{
    public string PolicyName { get; set; }
    public string PolicyExpression { get; set; }
    public string Description { get; set; }
    public bool IsSystemPolicy { get; set; }
    public virtual ICollection<Upload> Uploads { get; set; } = new List<Upload>();
}
