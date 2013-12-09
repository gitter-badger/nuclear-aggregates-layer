using System;
using System.IO;
using System.Net.Mime;

using DoubleGis.Erm.BL.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.LocalMessages
{
    public class CreateLocalMessageHandlerTest : UseModelEntityHandlerTestBase<OrganizationUnit, CreateLocalMessageRequest, EmptyResponse>
    {
        public CreateLocalMessageHandlerTest(IPublicService publicService, IAppropriateEntityProvider<OrganizationUnit> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(OrganizationUnit modelEntity, out CreateLocalMessageRequest request)
        {
            request = new CreateLocalMessageRequest
            {
                Entity = new LocalMessage
                {
                    EventDate = DateTime.UtcNow,
                    Status = (int)LocalMessageStatus.WaitForProcess,
                    OrganizationUnitId = modelEntity.Id,
                },
                FileName = "Test",
                IntegrationType = (int)IntegrationTypeExport.FirmsWithActiveOrdersToDgpp,
                Content = new MemoryStream(),
                ContentType = MediaTypeNames.Text.Xml,
            };

            return true;
        }
    }
}