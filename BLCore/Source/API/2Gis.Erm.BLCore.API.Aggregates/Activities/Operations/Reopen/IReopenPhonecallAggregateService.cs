using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Reopen
{
    public interface IReopenPhonecallAggregateService : IAggregateSpecificService<Phonecall, ReopenIdentity>
    {
        void Reopen(Phonecall phonecall);
    }
}
