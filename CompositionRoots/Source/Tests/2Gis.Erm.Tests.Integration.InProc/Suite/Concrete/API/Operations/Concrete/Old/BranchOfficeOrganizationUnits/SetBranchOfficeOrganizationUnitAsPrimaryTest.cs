using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.BranchOfficeOrganizationUnits;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.BranchOfficeOrganizationUnits
{
    public class SetBranchOfficeOrganizationUnitAsPrimaryTest :
        UseModelEntityHandlerTestBase<BranchOfficeOrganizationUnit, SetBranchOfficeOrganizationUnitAsPrimaryRequest, EmptyResponse>
    {
        public SetBranchOfficeOrganizationUnitAsPrimaryTest(IPublicService publicService,
                                                            IAppropriateEntityProvider<BranchOfficeOrganizationUnit> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override FindSpecification<BranchOfficeOrganizationUnit> ModelEntitySpec
        {
            get { return base.ModelEntitySpec && new FindSpecification<BranchOfficeOrganizationUnit>(x => !x.IsPrimary); }
        }

        protected override bool TryCreateRequest(BranchOfficeOrganizationUnit modelEntity,
                                                 out SetBranchOfficeOrganizationUnitAsPrimaryRequest request)
        {
            request = new SetBranchOfficeOrganizationUnitAsPrimaryRequest
                {
                    Id = modelEntity.Id
                };

            return true;
        }
    }
}