using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Georgaphy;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Building;

namespace DoubleGis.Erm.BL.Operations.Concrete.Integration.Import
{
    public sealed class ImportBuildingService : IImportBuildingService
    {
        private readonly IImportBuildingAggregateService _importBuildingAggregateService;
        private readonly IIntegrationLocalizationSettings _integrationLocalizationSettings;
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportBuildingService(IImportBuildingAggregateService importBuildingAggregateService,
                                     IIntegrationLocalizationSettings integrationLocalizationSettings,
                                     IIntegrationSettings integrationSettings,
                                     IMsCrmSettings msCrmSettings,
                                     IOperationScopeFactory scopeFactory)
        {
            _importBuildingAggregateService = importBuildingAggregateService;
            _integrationLocalizationSettings = integrationLocalizationSettings;
            _integrationSettings = integrationSettings;
            _msCrmSettings = msCrmSettings;
            _scopeFactory = scopeFactory;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var buildingServiceBusDtos = dtos.Cast<BuildingServiceBusDto>();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportBuildingIdentity>())
            {
                _importBuildingAggregateService.ImportBuildingFromServiceBus(buildingServiceBusDtos,
                                                                             _integrationLocalizationSettings.RegionalTerritoryLocaleSpecificWord,
                                                                             _msCrmSettings.IntegrationMode.HasFlag(MsCrmIntegrationMode.Database),
                                                                             _integrationSettings.UseWarehouseIntegration);

                scope.Complete();
            }
        }
    }
}