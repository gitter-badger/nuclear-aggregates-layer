using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DoubleGis.Erm.BLCore.API.OrderValidation;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    public sealed class OrderValidationRuleProvider : IOrderValidationRuleProvider
    {
        private readonly IOrderValidationRule[] _allValidationRules;

        private readonly IReadOnlyCollection<OrderValidationRuleGroup> _orderedValidationRuleGroupsSequence = new[]
            {
                OrderValidationRuleGroup.Generic,
                OrderValidationRuleGroup.AdvertisementMaterialsValidation,
                OrderValidationRuleGroup.ADPositionsValidation,
                OrderValidationRuleGroup.AdvertisementAmountValidation
            };

        public OrderValidationRuleProvider(
            // ReSharper disable ParameterTypeCanBeEnumerable.Local
            IOrderValidationRule[] allValidationRules
            // ReSharper restore ParameterTypeCanBeEnumerable.Local
            )
        {
            _allValidationRules = allValidationRules;
        }

        public IEnumerable<OrderValidationRuleContainer> GetAppropriateRules(ValidationType validationType)
        {
            throw new NotImplementedException();
        }
    }
}
