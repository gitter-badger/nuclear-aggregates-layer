using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Simplified.Dictionary.ContributionTypes
{
    public class ContributionTypeServiceTest : UseModelEntityTestBase<ContributionType>
    {
        private readonly IContributionTypeService _contributionTypeService;

        public ContributionTypeServiceTest(IAppropriateEntityProvider<ContributionType> appropriateEntityProvider,
                                           IContributionTypeService contributionTypeService) : base(appropriateEntityProvider)
        {
            _contributionTypeService = contributionTypeService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(ContributionType modelEntity)
        {
            modelEntity.DgppId = 1;
            _contributionTypeService.CreateOrUpdate(modelEntity);

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}