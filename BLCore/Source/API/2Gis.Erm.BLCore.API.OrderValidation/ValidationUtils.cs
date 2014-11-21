using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public static class ValidationUtils
    {
        public static bool IsOrderSpecific(this OrderValidationMessage orderValidationMessage)
        {
            return orderValidationMessage.OrderId != 0;
        }

        public static bool IsInvalid(this OrderValidationMessage orderValidationMessage)
        {
            return orderValidationMessage.Type == MessageType.Error || orderValidationMessage.Type == MessageType.Warning;
        }

        public static ValidationResult ToValidationResult(this IValidationResultsBrowser validationResultsBrowser)
        {
            var validatedOrders = new HashSet<long>();

            var errorsWithOrderSpecifed = new List<OrderValidationMessage>();
            var errorsWithoutOrders = new List<OrderValidationMessage>();

            foreach (var validator in validationResultsBrowser.ScheduledValidatorsSequence)
            {
                IReadOnlyDictionary<long, byte[]> validatorTargetOrders;
                IReadOnlyList<OrderValidationMessage> validatorResults;
                if (!validationResultsBrowser.TryGetValidatorReport(validator, out validatorTargetOrders, out validatorResults))
                {
                    throw new InvalidOperationException("Can't get report for validator. " + validator);
                }

                foreach (var orderId in validatorTargetOrders.Keys)
                {
                    validatedOrders.Add(orderId);
                }

                if (validatorResults == null)
                {   // �.�. �������� ����� rule ���� ��������, ������, ����������� �������� ��� � ����������, �������� �� ����� ������ �������� ����������, �.�. ���������� �������� �� ���������
                    // TODO {all, 01.10.2014}: �������� ����� ������� ����� ����� 
                    throw new NotSupportedException();
                }

                foreach (var result in validatorResults)
                {
                    if (result.IsOrderSpecific())
                    {
                        errorsWithOrderSpecifed.Add(result);
                    }
                    else
                    {
                        errorsWithoutOrders.Add(result);
                    }
                }
            }

            // ������� ������ �� ��������� � ������� � �����, �������� �������
            var messages = new OrderValidationMessage[errorsWithOrderSpecifed.Count + errorsWithoutOrders.Count];
            errorsWithOrderSpecifed.CopyTo(messages);
            errorsWithoutOrders.CopyTo(messages, errorsWithOrderSpecifed.Count);

            return new ValidationResult { OrderCount = validatedOrders.Count, Messages = messages };
        }
    }
}