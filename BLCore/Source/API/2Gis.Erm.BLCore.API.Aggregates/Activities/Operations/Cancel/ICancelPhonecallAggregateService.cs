﻿using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Cancel
{
    public interface ICancelPhonecallAggregateService : IAggregateSpecificService<Phonecall, CancelIdentity>
    {
        void Cancel(Phonecall phonecall);
    }
}
