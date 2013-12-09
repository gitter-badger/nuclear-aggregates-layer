using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Territories;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Territories
{
    public class SelectOrganizationUnitTerritoriesHandlerTest :
        UseModelEntityHandlerTestBase<OrganizationUnit, SelectOrganizationUnitTerritoriesRequest, SelectOrganizationUnitTerritoriesResponse>
    {
        public SelectOrganizationUnitTerritoriesHandlerTest(IPublicService publicService, IAppropriateEntityProvider<OrganizationUnit> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(OrganizationUnit modelEntity, out SelectOrganizationUnitTerritoriesRequest request)
        {
            request = new SelectOrganizationUnitTerritoriesRequest
                {
                    OrganizationUnitId = modelEntity.Id
                };

            return true;
        }
    }
}