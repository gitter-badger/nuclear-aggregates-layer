﻿using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IAssignAccountDetailAggregateService : IAggregateSpecificService<Account, AssignIdentity>
    {
        IEnumerable<ChangesDescriptor> Assign(AccountDetail entity, long ownerCode);
    }
}