using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.OrderProcessing
{
    public interface IOrderProcessingStrategy
    {
        void Validate(Order order);
        void Process(Order order);
        void FinishProcessing(Order order);
    }
}