using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Bills
{
    public class DeleteBillsHandlerTest : UseModelEntityHandlerTestBase<Order, DeleteBillsRequest, EmptyResponse>
    {
        public DeleteBillsHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Order> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override FindSpecification<Order> ModelEntitySpec
        {
            get
            {
                return base.ModelEntitySpec &&
                       new FindSpecification<Order>(o => o.WorkflowStepId == (int)OrderState.OnRegistration && o.Bills.Any(b => b.IsActive && !b.IsDeleted));
            }
        }

        protected override bool TryCreateRequest(Order modelEntity, out DeleteBillsRequest request)
        {
            request = new DeleteBillsRequest
                {
                    OrderId = modelEntity.Id
                };

            return true;
        }
    }
}