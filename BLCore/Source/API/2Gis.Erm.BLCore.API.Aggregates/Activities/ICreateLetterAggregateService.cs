using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface ICreateLetterAggregateService : IAggregateSpecificService<Letter, CreateIdentity>
    {
        void Create(Letter letter);
    }
}