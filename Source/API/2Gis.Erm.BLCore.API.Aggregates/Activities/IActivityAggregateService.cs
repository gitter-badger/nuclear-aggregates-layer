using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IActivityAggregateService<TActivity> : IAggregateRootRepository<TActivity> where TActivity : ActivityBase
    {
        long Create(TActivity activity);
        void Update(TActivity activity);
        void Delete(TActivity activity);
    }
}