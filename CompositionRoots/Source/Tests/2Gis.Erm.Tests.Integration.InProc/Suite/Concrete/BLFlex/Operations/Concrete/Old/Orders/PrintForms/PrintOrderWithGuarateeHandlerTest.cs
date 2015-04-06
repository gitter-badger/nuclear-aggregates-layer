using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.BLFlex.Operations.Concrete.Old.Orders.PrintForms
{
    public class PrintOrderWithGuarateeHandlerTest : UseModelEntityHandlerTestBase<Order, PrintOrderWithGuarateeRequest, StreamResponse>
    {
        public PrintOrderWithGuarateeHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Order> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(Order modelEntity, out PrintOrderWithGuarateeRequest request)
        {
            request = new PrintOrderWithGuarateeRequest
            {
                OrderId = modelEntity.Id,
            };

            return true;
        }

        protected override IResponseAsserter<StreamResponse> ResponseAsserter
        {
            get { return new StreamResponseAsserter(); }
        }
    }
}