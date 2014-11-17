using System;

using DoubleGis.Erm.BLCore.API.OrderValidation;

namespace DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions.Feedback
{
    public interface IOrderValidationOperationFeedback
    {
        void OperationStarted(ValidationParams validationParams);
        void OperationFailed(Exception exception);
        void OperationSucceeded();

        void ValidationStarted();
        void ValidationFailed(Exception exception);
        void ValidationSucceeded(int appropriateOrdersCount);

        void CachingStarted();
        void CachingFailed(Exception exception);
        void CachingSucceeded();

        void GroupStarted(OrderValidationRuleGroup ruleGroup);
        void GroupFailed(OrderValidationRuleGroup ruleGroup, Exception exception);
        void GroupSucceeded(OrderValidationRuleGroup ruleGroup, int validatedOrdersCount);

        void RuleStarted(Type ruleType);
        void RuleFailed(Type ruleType, Exception exception);
        void RuleSucceeded(Type ruleType, int validatedOrdersCount);
    }
}
