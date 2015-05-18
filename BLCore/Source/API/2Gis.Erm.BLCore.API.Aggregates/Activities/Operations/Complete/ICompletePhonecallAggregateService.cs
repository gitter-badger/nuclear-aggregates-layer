﻿using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Complete
{
    public interface ICompletePhonecallAggregateService : IAggregateSpecificOperation<Phonecall, CompleteIdentity>
    {
        void Complete(Phonecall phonecall);
    }
}
