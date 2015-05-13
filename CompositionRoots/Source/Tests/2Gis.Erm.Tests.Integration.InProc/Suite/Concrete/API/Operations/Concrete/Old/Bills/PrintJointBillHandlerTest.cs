using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Bills
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

        protected override IResponseAsserter<StreamResponse> ResponseAsserter
        {
            get { return new StreamResponseAsserter(); }
        }
    }
}