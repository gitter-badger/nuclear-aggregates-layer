using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IAssignLetterAggregateService : IAggregateSpecificOperation<Letter, AssignIdentity>
    {
        void Assign(Letter letter, long ownerCode);
    }
}