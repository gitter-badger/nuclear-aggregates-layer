using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary
{
    public interface IContributionTypeService : ISimplifiedModelConsumer
    {
        void CreateOrUpdate(ContributionType contributionType);
    }
}
