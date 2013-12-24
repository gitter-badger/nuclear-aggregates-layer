using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.BLFlex.Operations.Concrete.Old.Orders.PrintForms
{
    public class PrintOrderHandlerTest : UseModelEntityHandlerTestBase<Order, PrintOrderRequest, StreamResponse>
    {
        public PrintOrderHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Order> appropriateEntityProvider) : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(Order modelEntity, out PrintOrderRequest request)
        {
            request = new PrintOrderRequest
                {
                    LegalPersonProfileId = modelEntity.LegalPersonProfileId,
                    OrderId = modelEntity.Id,
                    PrintRegionalVersion = modelEntity.SourceOrganizationUnitId != modelEntity.DestOrganizationUnitId
                };

            return true;
        }

        protected override IResponseAsserter<StreamResponse> ResponseAsserter
        {
            get { return new StreamResponseAsserter(); }
        }
    }
}