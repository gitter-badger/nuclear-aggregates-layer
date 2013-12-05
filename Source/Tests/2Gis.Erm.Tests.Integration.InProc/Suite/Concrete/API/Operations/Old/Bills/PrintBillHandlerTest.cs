using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Old.Bills
{
    public class PrintBillHandlerTest : UseModelEntityHandlerTestBase<Bill, PrintBillRequest, StreamResponse>
    {
        public PrintBillHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Bill> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(Bill modelEntity, out PrintBillRequest request)
        {
            request = new PrintBillRequest
                {
                    Id = modelEntity.Id
                };

            return true;
        }

        protected override OrdinaryTestResult AssertResponse(StreamResponse response)
        {
            return Result.When(response)
                         .Then(r => r.Stream.Should().NotBeNull());
        }
    }
}