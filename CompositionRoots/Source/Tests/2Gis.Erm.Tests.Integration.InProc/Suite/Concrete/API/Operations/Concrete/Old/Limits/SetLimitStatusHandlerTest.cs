using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Limits
{
    public class SetLimitStatusHandlerTest : UseModelEntityHandlerTestBase<Limit, SetLimitStatusRequest, EmptyResponse>
    {
        public SetLimitStatusHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Limit> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override FindSpecification<Limit> ModelEntitySpec
        {
            get 
            { 
                return base.ModelEntitySpec && new FindSpecification<Limit>(l => l.Status != (int)LimitStatus.Opened
                    && !l.Account.Orders
                        .Any(o => o.BeginDistributionDate <= l.StartPeriodDate && o.EndDistributionDateFact >= l.EndPeriodDate
                        && (o.WorkflowStepId == (int)OrderState.Approved || o.WorkflowStepId == (int)OrderState.OnTermination || o.WorkflowStepId == (int)OrderState.Archive)));
            }
        }

        protected override bool TryCreateRequest(Limit modelEntity, out SetLimitStatusRequest request)
        {
            request = new SetLimitStatusRequest
                {
                    Status = LimitStatus.Opened,
                    LimitReplicationCodes = new[] { modelEntity.ReplicationCode }
                };

            return true;
        }
    }
}