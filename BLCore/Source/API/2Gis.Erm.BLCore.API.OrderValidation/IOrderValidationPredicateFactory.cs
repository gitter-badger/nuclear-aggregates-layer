namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IOrderValidationPredicateFactory
    {
        OrderValidationPredicate CreatePredicate(ValidationParams validationParams);
    }
}