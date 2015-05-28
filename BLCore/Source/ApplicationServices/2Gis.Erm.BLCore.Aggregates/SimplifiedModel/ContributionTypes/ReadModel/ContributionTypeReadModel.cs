using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ContributionTypes.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.ContributionTypes.ReadModel
{
    public class ContributionTypeReadModel : IContributionTypeReadModel
    {
        private readonly ISecureFinder _finder;

        public ContributionTypeReadModel(ISecureFinder finder)
        {
            _finder = finder;
        }

        public string GetContributionTypeName(long contributionTypeId)
        {
            return _finder.FindObsolete(Specs.Find.ById<ContributionType>(contributionTypeId)).Select(x => x.Name).Single();
        }
    }
}