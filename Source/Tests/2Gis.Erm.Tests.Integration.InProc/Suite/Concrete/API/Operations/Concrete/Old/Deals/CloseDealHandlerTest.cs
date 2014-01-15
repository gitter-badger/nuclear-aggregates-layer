using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Deals
{
    public class CloseDealHandlerTest : UseModelEntityHandlerTestBase<Deal, CloseDealRequest, EmptyResponse>
    {
        public CloseDealHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Deal> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override FindSpecification<Deal> ModelEntitySpec
        {
            get
            {
                return base.ModelEntitySpec &&
                       new FindSpecification<Deal>(
                           d =>
                           d.Orders.All(
                               o => o.IsActive && !o.IsDeleted && o.WorkflowStepId == (int)OrderState.Rejected || o.WorkflowStepId == (int)OrderState.Archive));
            }
        }

        protected override bool TryCreateRequest(Deal modelEntity, out CloseDealRequest request)
        {
            request = new CloseDealRequest()
                {
                    CloseReason = CloseDealReason.Reason1,
                    Comment = "Test",
                    Id = modelEntity.Id
                };

            return true;
        }
    }
}