using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IValidationResultsBrowser
    {
        // COMMENT {i.maslennikov, 08.10.2014}: Это знание не сильно подходит этой абстракции
        IReadOnlyList<ValidatorDescriptor> ScheduledValidatorsSequence { get; }

        bool TryGetValidatorReport(ValidatorDescriptor validatorDescriptor,
                                   out IReadOnlyDictionary<long, byte[]> validatorTargetOrders,
                                   out IReadOnlyList<OrderValidationMessage> validatorResults);
    }
}