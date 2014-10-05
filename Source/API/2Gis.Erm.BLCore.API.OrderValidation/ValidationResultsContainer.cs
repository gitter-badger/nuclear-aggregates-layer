using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public sealed class ValidationResultsContainer : IValidationResultsBrowser
    {
        private readonly Dictionary<ValidatorDescriptor, List<OrderValidationMessage>> _validationResultsByValidators =
            new Dictionary<ValidatorDescriptor, List<OrderValidationMessage>>();

        private readonly List<ValidatorDescriptor> _scheduledValidatorsSequence = new List<ValidatorDescriptor>();

        private readonly Dictionary<ValidatorDescriptor, IReadOnlyDictionary<long, byte[]>> _validationTargetOrdersByValidators =
            new Dictionary<ValidatorDescriptor, IReadOnlyDictionary<long, byte[]>>();
        
        IReadOnlyList<ValidatorDescriptor> IValidationResultsBrowser.ScheduledValidatorsSequence
        {
            get { return _scheduledValidatorsSequence; }
        }

        public void AttachTargets(ValidatorDescriptor validatorDescriptor, IReadOnlyDictionary<long, byte[]> validatorTargetOrders)
        {
            _validationTargetOrdersByValidators.Add(validatorDescriptor, validatorTargetOrders);
            _scheduledValidatorsSequence.Add(validatorDescriptor);
        }

        public void AttachResults(ValidatorDescriptor validatorDescriptor, IEnumerable<OrderValidationMessage> orderValidationMessages)
        {
            List<OrderValidationMessage> results;
            if (!_validationResultsByValidators.TryGetValue(validatorDescriptor, out results))
            {
                results = new List<OrderValidationMessage>();
                _validationResultsByValidators.Add(validatorDescriptor, results);
            }

            foreach (var message in orderValidationMessages)
            {
                message.RuleCode = validatorDescriptor.RuleCode;
                results.Add(message);
            }
        }

        bool IValidationResultsBrowser.TryGetValidatorReport(ValidatorDescriptor validatorDescriptor,
                                          out IReadOnlyDictionary<long, byte[]> validatorTargetOrders,
                                          out IReadOnlyList<OrderValidationMessage> validatorResults)
        {
            validatorResults = new List<OrderValidationMessage>();

            if (!_validationTargetOrdersByValidators.TryGetValue(validatorDescriptor, out validatorTargetOrders))
            {
                validatorTargetOrders = new Dictionary<long, byte[]>();
                return false;
            }

            List<OrderValidationMessage> results;
            if (_validationResultsByValidators.TryGetValue(validatorDescriptor, out results))
            {
                validatorResults = results;
            }

            return true;
        }
    }
}