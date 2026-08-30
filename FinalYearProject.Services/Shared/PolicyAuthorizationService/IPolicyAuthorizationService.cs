using System;
using System.Collections.Generic;
using System.Text;

namespace FinalYearProject.Services.Shared.PolicyAuthorizationService;

public interface IPolicyAuthorizationService
{
    PolicyAuthorizationResult CanBypassPolicy(string PolicyExpression, IEnumerable<Guid> userAttributeIds, bool isSuperAdmin);

    PolicyAuthorizationResult CanCreatePolicy(PolicyNodeRequest rules, IEnumerable<Guid> userAttributeIds, bool isSuperAdmin);
}
