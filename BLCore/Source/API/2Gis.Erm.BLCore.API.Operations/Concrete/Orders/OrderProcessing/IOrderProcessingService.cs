using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.OrderProcessing
{
    public interface IOrderProcessingService
    {
        IOrderProcessingStrategy[] EvaluateProcessingStrategies(IUseCaseResumeContext<EditOrderRequest> resumeContext, IOperationScope operationScope);
        void ExecuteProcessingStrategies(IOrderProcessingStrategy[] strategies, Order order);
    }
}