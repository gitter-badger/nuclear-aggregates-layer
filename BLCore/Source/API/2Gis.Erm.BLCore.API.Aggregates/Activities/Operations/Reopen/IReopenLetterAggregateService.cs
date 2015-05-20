using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Reopen
{
    public interface IReopenLetterAggregateService : IAggregateSpecificOperation<Letter, ReopenIdentity>
    {
        void Reopen(Letter letter);
    }
}
