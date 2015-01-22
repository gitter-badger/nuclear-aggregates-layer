using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface ICreateLetterAggregateService : IAggregateSpecificOperation<Letter, CreateIdentity>
    {
        void Create(Letter letter);
    }
}