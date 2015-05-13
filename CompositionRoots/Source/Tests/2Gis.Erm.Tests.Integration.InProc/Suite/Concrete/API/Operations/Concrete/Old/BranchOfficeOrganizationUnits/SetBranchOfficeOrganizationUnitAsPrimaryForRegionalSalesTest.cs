using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.BranchOfficeOrganizationUnits;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.BranchOfficeOrganizationUnits
{
    public class SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesTest :
        UseModelEntityHandlerTestBase<BranchOfficeOrganizationUnit, SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesRequest, EmptyResponse>
    {
        public SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesTest(IPublicService publicService,
                                                                            IAppropriateEntityProvider<BranchOfficeOrganizationUnit> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override FindSpecification<BranchOfficeOrganizationUnit> ModelEntitySpec
        {
            get { return base.ModelEntitySpec && new FindSpecification<BranchOfficeOrganizationUnit>(x => !x.IsPrimaryForRegionalSales); }
        }

        protected override bool TryCreateRequest(BranchOfficeOrganizationUnit modelEntity,
                                                 out SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesRequest request)
        {
            request = new SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesRequest
                {
                    Id = modelEntity.Id
                };

            return true;
        }
    }
}