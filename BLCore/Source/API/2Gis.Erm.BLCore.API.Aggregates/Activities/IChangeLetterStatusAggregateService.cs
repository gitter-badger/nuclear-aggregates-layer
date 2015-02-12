using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.CancelActivity;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IChangeLetterStatusAggregateService : IAggregateSpecificOperation<Letter, CancelActivityIdentity>
    {
        void Change(Letter letter, ActivityStatus status);
    }
}
