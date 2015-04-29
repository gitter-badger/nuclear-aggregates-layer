using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IAssignPhonecallAggregateService : IAggregateSpecificOperation<Phonecall, AssignIdentity>
    {
        void Assign(Phonecall phonecall, long ownerCode);
    }
}