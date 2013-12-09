using System;

using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Reports;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Reports
{
    public class PlanningReportHandlerTest : UseModelEntityHandlerTestBase<OrganizationUnit, PlanningReportRequest, StreamResponse>
    {
        public PlanningReportHandlerTest(IPublicService publicService, IAppropriateEntityProvider<OrganizationUnit> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(OrganizationUnit modelEntity, out PlanningReportRequest request)
        {
            var date = DateTime.UtcNow.GetPrevMonthFirstDate();
            request = new PlanningReportRequest
                {
                    OrganizationUnitId = modelEntity.Id,
                    PlanningMonth = date
                };

            return true;
        }
    }
}