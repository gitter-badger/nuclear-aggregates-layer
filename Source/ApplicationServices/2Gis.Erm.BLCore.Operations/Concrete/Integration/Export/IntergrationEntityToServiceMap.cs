using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export
{
    public static class IntergrationEntityToServiceMap
    {
        private static readonly Dictionary<EntityName, IntegrationService> Map = new Dictionary<EntityName, IntegrationService>
            {
                { EntityName.ExportFlowCardExtensionsCardCommercial, IntegrationService.ExportFlowCardExtensionsCardCommercial },
                { EntityName.ExportFlowFinancialDataLegalEntity, IntegrationService.ExportFlowFinancialDataLegalEntity },
                { EntityName.ExportFlowOrdersAdvMaterial, IntegrationService.ExportFlowOrdersAdvMaterial },
                { EntityName.ExportFlowOrdersOrder, IntegrationService.ExportFlowOrdersOrder },
                { EntityName.ExportFlowOrdersResource, IntegrationService.ExportFlowOrdersResource },
                { EntityName.ExportFlowOrdersTheme, IntegrationService.ExportFlowOrdersTheme },
                { EntityName.ExportFlowOrdersThemeBranch, IntegrationService.ExportFlowOrdersThemeBranch },
                { EntityName.ExportFlowFinancialDataClient, IntegrationService.ExportFlowFinancialDataClient },
                { EntityName.ExportFlowPriceListsPriceList, IntegrationService.ExportFlowPriceListsPriceList },
                { EntityName.ExportFlowPriceListsPriceListPosition, IntegrationService.ExportFlowPriceListsPriceListPosition },
                { EntityName.ExportToMsCrmHotClients, IntegrationService.CreateHotClientTask },
                { EntityName.ImportedFirmAddress, IntegrationService.ImportFirmAddressNames },
                { EntityName.ExportFlowOrdersInvoice, IntegrationService.ExportFlowOrdersInvoice },
            };

        public static IntegrationService AsIntegrationService(this EntityName entityName)
        {
            IntegrationService integrationService;
            if (!entityName.TryGetIntegrationService(out integrationService))
            {
                throw new ArgumentException(string.Format("Cannot find type mapped to EntityName {0}", entityName));
            }

            return integrationService;
        }

        public static bool TryGetIntegrationService(this EntityName entityName, out IntegrationService integrationService)
        {
            integrationService = IntegrationService.Undefined;
            return !entityName.IsVirtual() && Map.TryGetValue(entityName, out integrationService);
        }
    }
}