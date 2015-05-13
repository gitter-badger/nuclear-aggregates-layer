using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.BLFlex.Operations.Concrete.OrderProcessingRequests
{
    public class CancelOrderProcessingRequestOperationServiceTest : UseModelEntityTestBase<OrderProcessingRequest>
    {
        private readonly ICancelOrderProcessingRequestOperationService _cancelOrderProcessingRequestOperationService;

        public CancelOrderProcessingRequestOperationServiceTest(IAppropriateEntityProvider<OrderProcessingRequest> appropriateEntityProvider,
                                                                ICancelOrderProcessingRequestOperationService cancelOrderProcessingRequestOperationService)
            : base(appropriateEntityProvider)
        {
            _cancelOrderProcessingRequestOperationService = cancelOrderProcessingRequestOperationService;
        }

        protected override FindSpecification<OrderProcessingRequest> ModelEntitySpec
        {
            get
            {
                return base.ModelEntitySpec &&
                       new FindSpecification<OrderProcessingRequest>(
                           r => r.State != OrderProcessingRequestState.Completed && r.State != OrderProcessingRequestState.Cancelled);
            }
        }

        protected override OrdinaryTestResult ExecuteWithModel(OrderProcessingRequest modelEntity)
        {
            return Result.When(() => _cancelOrderProcessingRequestOperationService.CancelRequest(modelEntity.Id)).Then(r => r.Status.Should().Be(TestResultStatus.Succeeded));
        }
    }
}