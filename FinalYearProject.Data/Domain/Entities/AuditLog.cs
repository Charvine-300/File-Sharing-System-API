using FinalYearProject.Data.Domain.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinalYearProject.Data.Domain.Entities;

public class AuditLog: BaseEntity
{
    public string Action { get; set; }
    public string? Description { get; set; }
    public string Actor { get; set; } // this will be th user's full name
    public ActionType Type { get; set; }
}
