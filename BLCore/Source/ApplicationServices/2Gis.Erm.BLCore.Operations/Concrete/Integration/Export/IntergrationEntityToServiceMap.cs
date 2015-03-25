using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export
{
    public static class IntergrationEntityToServiceMap
    {
        private static readonly Dictionary<IEntityType, IntegrationService> Map = new Dictionary<IEntityType, IntegrationService>
            {
                { EntityType.Instance.ExportFlowCardExtensionsCardCommercial(), IntegrationService.ExportFlowCardExtensionsCardCommercial },
                { EntityType.Instance.ExportFlowFinancialDataLegalEntity(), IntegrationService.ExportFlowFinancialDataLegalEntity },
                { EntityType.Instance.ExportFlowOrdersAdvMaterial(), IntegrationService.ExportFlowOrdersAdvMaterial },
                { EntityType.Instance.ExportFlowOrdersOrder(), IntegrationService.ExportFlowOrdersOrder },
                { EntityType.Instance.ExportFlowOrdersResource(), IntegrationService.ExportFlowOrdersResource },
                { EntityType.Instance.ExportFlowOrdersTheme(), IntegrationService.ExportFlowOrdersTheme },
                { EntityType.Instance.ExportFlowOrdersThemeBranch(), IntegrationService.ExportFlowOrdersThemeBranch },
                { EntityType.Instance.ExportFlowFinancialDataClient(), IntegrationService.ExportFlowFinancialDataClient },
                { EntityType.Instance.ExportFlowFinancialDataDebitsInfoInitial(), IntegrationService.ExportFlowFinancialDataDebitsInfoInitial },
                { EntityType.Instance.ExportFlowPriceListsPriceList(), IntegrationService.ExportFlowPriceListsPriceList },
                { EntityType.Instance.ExportFlowPriceListsPriceListPosition(), IntegrationService.ExportFlowPriceListsPriceListPosition },
                { EntityType.Instance.ImportedFirmAddress(), IntegrationService.ImportFirmAddressNames },
                { EntityType.Instance.ExportFlowOrdersInvoice(), IntegrationService.ExportFlowOrdersInvoice },
                { EntityType.Instance.ExportFlowNomenclaturesNomenclatureElement(), IntegrationService.ExportFlowNomenclaturesNomenclatureElement },
                { EntityType.Instance.ExportFlowNomenclaturesNomenclatureElementRelation(), IntegrationService.ExportFlowNomenclaturesNomenclatureElementRelation },
                { EntityType.Instance.ExportFlowDeliveryDataLetterSendRequest(), IntegrationService.ExportFlowDeliveryDataLetterSendRequest },
                { EntityType.Instance.ExportFlowOrdersDenialReason(), IntegrationService.ExportFlowOrdersDenialReason },
            };

        public static IntegrationService AsIntegrationService(this IEntityType entityName)
        {
            IntegrationService integrationService;
            if (!entityName.TryGetIntegrationService(out integrationService))
            {
                throw new ArgumentException(string.Format("Cannot find type mapped to EntityName {0}", entityName));
            }

            return integrationService;
        }

        public static bool TryGetIntegrationService(this IEntityType entityName, out IntegrationService integrationService)
        {
            integrationService = IntegrationService.Undefined;
            return !entityName.IsVirtual() && Map.TryGetValue(entityName, out integrationService);
        }
    }
}