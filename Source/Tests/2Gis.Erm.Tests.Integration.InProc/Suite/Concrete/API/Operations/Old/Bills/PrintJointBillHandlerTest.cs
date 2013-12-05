using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Old.Bills
{
    public class PrintJointBillHandlerTest : UseModelEntityHandlerTestBase<Bill, PrintJointBillRequest, StreamResponse>
    {
        private readonly IAppropriateEntityProvider<Bill> _appropriateEntityProvider;

        public PrintJointBillHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Bill> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
            _appropriateEntityProvider = appropriateEntityProvider;
        }

        protected override bool TryCreateRequest(Bill modelEntity, out PrintJointBillRequest request)
        {
            request = null;

            var bills = _appropriateEntityProvider.Get(ModelEntitySpec && new FindSpecification<Bill>(b => b.Id != modelEntity.Id), 5);
            if (!bills.Any())
            {
                return false;
            }

            request = new PrintJointBillRequest
                {
                    BillId = modelEntity.Id,
                    RelatedOrdersId = bills.Select(x => x.OrderId).ToArray()
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