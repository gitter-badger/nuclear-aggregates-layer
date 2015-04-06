using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Shared;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.Crosscutting
{
    public class UkraineEvaluateOrderNumberService : IEvaluateOrderNumberService, IUkraineAdapted
    {
        public const string OrderNumberTemplate = "БЗ_{0}-{1}-{2}";
        private readonly OrderNumberEvaluator _numberEvaluator;

        public UkraineEvaluateOrderNumberService(IEnumerable<IOrderNumberGenerationStrategy> strategies)
        {
            _numberEvaluator = new OrderNumberEvaluator(strategies);
        }

        public string Evaluate(string currentOrderNumber,
                               string sourceOrganizationUnitSyncCode1C,
                               string destinationOrganizationUnitSyncCode1C,
                               long? generatedOrderIndex,
                               OrderType orderType)
        {
            return _numberEvaluator.Evaluate(OrderNumberTemplate,
                                             currentOrderNumber,
                                             sourceOrganizationUnitSyncCode1C,
                                             destinationOrganizationUnitSyncCode1C,
                                             generatedOrderIndex);
        }

        public string EvaluateRegional(string currentOrderNumber,
                                       string sourceOrganizationUnitSyncCode1C,
                                       string destinationOrganizationUnitSyncCode1C,
                                       long? generatedOrderIndex)
        {
            return _numberEvaluator.Evaluate(OrderNumberTemplate,
                                             currentOrderNumber,
                                             sourceOrganizationUnitSyncCode1C,
                                             destinationOrganizationUnitSyncCode1C,
                                             generatedOrderIndex);
        }
    }
}