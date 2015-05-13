using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.BLFlex.Operations.Concrete.OrderProcessingRequests
{
    public class ProcessOrderCreationRequestSingleOperationTest : UseModelEntityTestBase<OrderProcessingRequest>
    {
        private readonly IProcessOrderCreationRequestSingleOperation _operation;

        public ProcessOrderCreationRequestSingleOperationTest(IProcessOrderCreationRequestSingleOperation operation,
                                                              IAppropriateEntityProvider<OrderProcessingRequest> appropriateEntityProvider)
            : base(appropriateEntityProvider)
        {
            _operation = operation;
        }

        protected override FindSpecification<OrderProcessingRequest> ModelEntitySpec
        {
            get
            {
                return base.ModelEntitySpec &&
                       new FindSpecification<OrderProcessingRequest>(
                           x => x.RequestType == OrderProcessingRequestType.CreateOrder && x.State == OrderProcessingRequestState.Pending);
            }
        }

        protected override OrdinaryTestResult ExecuteWithModel(OrderProcessingRequest modelEntity)
        {
            return Result.When(_operation.ProcessSingle(modelEntity.Id))
                         .Then(r => r.Order.Should().NotBeNull());
        }
    }
}