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
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportBuildingService(IIntegrationLocalizationSettings integrationLocalizationSettings,
                                     IMsCrmSettings msCrmSettings,
                                     IImportBuildingAggregateService importBuildingAggregateService,
                                     IOperationScopeFactory scopeFactory)
        {
            _integrationLocalizationSettings = integrationLocalizationSettings;
            _msCrmSettings = msCrmSettings;
            _importBuildingAggregateService = importBuildingAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var buildingServiceBusDtos = dtos.Cast<BuildingServiceBusDto>();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportBuildingIdentity>())
            {
                _importBuildingAggregateService.ImportBuildingFromServiceBus(buildingServiceBusDtos,
                                                                             _integrationLocalizationSettings.RegionalTerritoryLocaleSpecificWord,
                                                                             _msCrmSettings.EnableReplication);

                scope.Complete();
            }
        }
    }
}