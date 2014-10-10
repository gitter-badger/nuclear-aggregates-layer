using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IValidationResultsBrowser
    {
        IReadOnlyList<ValidatorDescriptor> ScheduledValidatorsSequence { get; }

        bool TryGetValidatorReport(ValidatorDescriptor validatorDescriptor,
                                   out IReadOnlyDictionary<long, byte[]> validatorTargetOrders,
                                   out IReadOnlyList<OrderValidationMessage> validatorResults);
    }
}