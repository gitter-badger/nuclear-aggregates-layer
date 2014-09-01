using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.BLFlex.Operations.Concrete.Old.Orders.PrintForms
{
    public class PrintOrderHandlerTest : UseModelEntityHandlerTestBase<Order, Request, StreamResponse>
    {
        public PrintOrderHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Order> appropriateEntityProvider) : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(Order modelEntity, out Request request)
        {
            if (modelEntity.SourceOrganizationUnitId != modelEntity.DestOrganizationUnitId)
            {
                request = new PrintRegionalOrderRequest
                {
                    OrderId = modelEntity.Id,
                };
            }
            else
            {
                request = new PrintOrderRequest
                {
                    OrderId = modelEntity.Id,
                };
            }



            return true;
        }

        protected override IResponseAsserter<StreamResponse> ResponseAsserter
        {
            get { return new StreamResponseAsserter(); }
        }
    }
}