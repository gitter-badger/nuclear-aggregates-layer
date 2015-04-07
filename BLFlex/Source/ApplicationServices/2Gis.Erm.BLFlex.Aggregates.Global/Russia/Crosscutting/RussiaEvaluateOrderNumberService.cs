using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Shared;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Crosscutting
{
    public class RussiaEvaluateOrderNumberService : IEvaluateOrderNumberService, IRussiaAdapted
    {
        private const string OrderNumberTemplate = "БЗ_{0}-{1}-{2}";
        private const string RegionalOrderNumberTemplate = "ОФ_{0}-{1}-{2}";
        private const string AdvAgenciesTemplatePostfix = "-РА";
        private readonly IEnumerable<IOrderNumberGenerationStrategy> _strategies;

        public RussiaEvaluateOrderNumberService(IEnumerable<IOrderNumberGenerationStrategy> strategies)
        {
            _strategies = strategies;
        }

        public string Evaluate(string currentOrderNumber,
                               string sourceOrganizationUnitSyncCode1C,
                               string destinationOrganizationUnitSyncCode1C,
                               long? generatedOrderIndex,
                               OrderType orderType)
        {
            return _strategies.Execute(GetTemplate(orderType),
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
            return _strategies.Execute(GetRegionalTemplate(),
                                       currentOrderNumber,
                                       sourceOrganizationUnitSyncCode1C,
                                       destinationOrganizationUnitSyncCode1C,
                                       generatedOrderIndex);
        }

        private string GetTemplate(OrderType orderType)
        {
            if (orderType == OrderType.AdvertisementAgency)
            {
                return OrderNumberTemplate + AdvAgenciesTemplatePostfix;
            }

            return OrderNumberTemplate;
        }

        private string GetRegionalTemplate()
        {
            return RegionalOrderNumberTemplate;
        }
    }
}