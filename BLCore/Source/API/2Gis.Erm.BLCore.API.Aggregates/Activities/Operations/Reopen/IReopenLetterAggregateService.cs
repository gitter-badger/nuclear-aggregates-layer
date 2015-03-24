using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Reopen;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Reopen
{
    public interface IReopenLetterAggregateService : IAggregateSpecificOperation<Letter, ReopenIdentity>
    {
        void Reopen(Letter letter);
    }
}
