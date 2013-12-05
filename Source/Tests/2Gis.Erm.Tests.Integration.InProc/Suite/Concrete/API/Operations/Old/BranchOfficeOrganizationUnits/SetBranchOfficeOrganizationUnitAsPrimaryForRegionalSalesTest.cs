﻿using DoubleGis.Erm.BL.API.Operations.Concrete.Old.BranchOfficeOrganizationUnits;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Old.BranchOfficeOrganizationUnits
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