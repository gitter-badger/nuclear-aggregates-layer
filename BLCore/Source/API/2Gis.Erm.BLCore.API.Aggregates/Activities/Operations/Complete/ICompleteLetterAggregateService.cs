using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Complete;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Complete
{
    public interface ICompleteLetterAggregateService : IAggregateSpecificOperation<Letter, CompleteIdentity>
    {
        void Complete(Letter letter);
    }
}
