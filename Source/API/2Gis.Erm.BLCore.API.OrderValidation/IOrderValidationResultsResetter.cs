namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IOrderValidationResultsResetter
    {
        void SetGroupAsInvalid(long orderId, OrderValidationRuleGroup orderValidationRuleGroup);
    }
}