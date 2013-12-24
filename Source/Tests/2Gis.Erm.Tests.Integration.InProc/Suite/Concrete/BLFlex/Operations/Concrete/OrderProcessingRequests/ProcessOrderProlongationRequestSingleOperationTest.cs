using DoubleGis.Erm.BL.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.BLFlex.Operations.Concrete.OrderProcessingRequests
{
    public class ProcessOrderProlongationRequestSingleOperationTest : UseModelEntityTestBase<OrderProcessingRequest>
    {
        private readonly IProcessOrderProlongationRequestSingleOperation _processOrderProlongationRequestSingleOperation;

        public ProcessOrderProlongationRequestSingleOperationTest(
            IProcessOrderProlongationRequestSingleOperation processOrderProlongationRequestSingleOperation,
            IAppropriateEntityProvider<OrderProcessingRequest> appropriateEntityProvider)
            : base(appropriateEntityProvider)
        {
            _processOrderProlongationRequestSingleOperation = processOrderProlongationRequestSingleOperation;
        }

        protected override FindSpecification<OrderProcessingRequest> ModelEntitySpec
        {
            get
            {
                return base.ModelEntitySpec &&
                       new FindSpecification<OrderProcessingRequest>(
                           x => x.RequestType == (int)OrderProcessingRequestType.ProlongateOrder && x.State == (int)OrderProcessingRequestState.Pending);
            }
        }

        protected override OrdinaryTestResult ExecuteWithModel(OrderProcessingRequest modelEntity)
        {
            return Result.When(_processOrderProlongationRequestSingleOperation.ProcessSingle(modelEntity.Id))
                         .Then(r => r.OrderId.Should().NotBe(0));
        }
    }
}