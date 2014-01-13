namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IOrderValidationService  
    {
        ValidateOrdersResult ValidateOrders(OrderValidationPredicate filterPredicate, ValidateOrdersRequest request);
    }
}
