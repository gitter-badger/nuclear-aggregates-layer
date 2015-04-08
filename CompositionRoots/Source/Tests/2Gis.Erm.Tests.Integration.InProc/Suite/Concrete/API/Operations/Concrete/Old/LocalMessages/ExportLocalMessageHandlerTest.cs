using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.LocalMessages
{
    public class ExportLocalMessageHandlerTest : UseModelEntityHandlerTestBase<OrganizationUnit, ExportLocalMessageRequest, Response>
    {
        public ExportLocalMessageHandlerTest(IPublicService publicService, IAppropriateEntityProvider<OrganizationUnit> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(OrganizationUnit modelEntity, out ExportLocalMessageRequest request)
        {
            var periodStart = DateTime.UtcNow.AddMonths(-1).GetFirstDateOfMonth();
            request = new ExportLocalMessageRequest
                {
                    OrganizationUnitId = modelEntity.Id,
                    PeriodStart = periodStart,
                    IntegrationType = IntegrationTypeExport.LegalPersonsTo1C
                };

            return true;
        }
    }
}