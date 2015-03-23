using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting
{
    public class EvaluateOrderNumberService : IEvaluateOrderNumberService
    {
        private readonly IOrderNumberTemplatesProvider _numberTemplateProvider;
        private readonly IOrderNumberGenerationStrategiesProvider _generationStrategiesProvider;

        public EvaluateOrderNumberService(IOrderNumberTemplatesProvider numberTemplateProvider, IOrderNumberGenerationStrategiesProvider generationStrategiesProvider)
        {
            _numberTemplateProvider = numberTemplateProvider;
            _generationStrategiesProvider = generationStrategiesProvider;
        }

        public string Evaluate(string currentOrderNumber,
                               string sourceOrganizationUnitSyncCode1C,
                               string destinationOrganizationUnitSyncCode1C,
                               long? generatedOrderIndex,
                               OrderType orderType)
        {
            var orderNumberEvaluator = new OrderNumberEvaluator(_numberTemplateProvider.GetTemplate(orderType), _generationStrategiesProvider.GetStrategies());
            return orderNumberEvaluator.Evaluate(currentOrderNumber,
                                                 sourceOrganizationUnitSyncCode1C,
                                                 destinationOrganizationUnitSyncCode1C,
                                                 generatedOrderIndex);
        }

        public string EvaluateRegional(string currentOrderNumber,
                                       string sourceOrganizationUnitSyncCode1C,
                                       string destinationOrganizationUnitSyncCode1C,
                                       long? generatedOrderIndex)
        {
            var orderRegionalNumberEvaluator = new OrderNumberEvaluator(_numberTemplateProvider.GetRegionalTemplate(), _generationStrategiesProvider.GetStrategies());
            return orderRegionalNumberEvaluator.Evaluate(currentOrderNumber,
                                                         sourceOrganizationUnitSyncCode1C,
                                                         destinationOrganizationUnitSyncCode1C,
                                                         generatedOrderIndex);
        }
    }
}