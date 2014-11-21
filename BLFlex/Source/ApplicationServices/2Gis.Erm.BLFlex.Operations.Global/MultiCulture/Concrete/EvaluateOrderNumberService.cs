using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete
{
    // TODO {a.rechkalov, 07.04.2014}: есть ощущение, что эти классы нужно перенести в BL (не flex) и все же разделить по разным файлам
    // COMMENT {y.baranihin, 08.04.2014}: разделил, но перемещение приводит к цепочке обновления зависимостей
    //                                    кроме того, если перемещать, то сразу в DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting, где находятся остальные подобные сервисы
    // COMMENT {a.rechkalov, 09.04.2014}: когда-нибудь переместить код все же нужно будет
    public class EvaluateOrderNumberService : IEvaluateOrderNumberService
    {
        private readonly OrderNumberEvaluator _orderNumberEvaluator;
        private readonly OrderNumberEvaluator _orderRegionalNumberEvaluator;

        public EvaluateOrderNumberService(string numberTemplate, string regionalNumberTemplate, IEnumerable<OrderNumberGenerationStrategy> generationStrategies)
        {
            _orderNumberEvaluator = new OrderNumberEvaluator(numberTemplate, generationStrategies);
            _orderRegionalNumberEvaluator = new OrderNumberEvaluator(regionalNumberTemplate, generationStrategies);
        }

        public string Evaluate(string currentOrderNumber, string sourceOrganizationUnitSyncCode1C, string destinationOrganizationUnitSyncCode1C, long? generatedOrderIndex)
        {
            return _orderNumberEvaluator.Evaluate(currentOrderNumber,
                                                  sourceOrganizationUnitSyncCode1C,
                                                  destinationOrganizationUnitSyncCode1C,
                                                  generatedOrderIndex);
        }

        public string EvaluateRegional(string currentOrderNumber,
                                       string sourceOrganizationUnitSyncCode1C,
                                       string destinationOrganizationUnitSyncCode1C,
                                       long? generatedOrderIndex)
        {
            return _orderRegionalNumberEvaluator.Evaluate(currentOrderNumber,
                                                          sourceOrganizationUnitSyncCode1C,
                                                          destinationOrganizationUnitSyncCode1C,
                                                          generatedOrderIndex);
        }
    }
}
