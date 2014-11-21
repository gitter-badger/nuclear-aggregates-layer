using System;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IOrderValidationRuleFactory
    {
        IOrderValidationRule Create(Type orderValidationRuleType);
    }
}
