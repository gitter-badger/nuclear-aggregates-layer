using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class ReleaseNotExistsOrderValidationRule : OrderValidationRuleBase<SingleValidationRuleContext>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;

        public ReleaseNotExistsOrderValidationRule(ISubRequestProcessor subRequestProcessor)
        {
            _subRequestProcessor = subRequestProcessor;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(SingleValidationRuleContext ruleContext)
        {
            var response = 
                    (CheckOrderReleasePeriodResponse)_subRequestProcessor.HandleSubRequest(
                        new CheckOrderReleasePeriodRequest
                            {
                                    OrderId = ruleContext.ValidationParams.OrderId,
                                InProgressOnly = false,
                            },
                        null);

            if (!response.Success && ruleContext.ValidationParams.CurrentOrderState != OrderState.Approved)
            {
                return new[] { response.Message };  
            } 

            return Enumerable.Empty<OrderValidationMessage>();
        }
    }
}
