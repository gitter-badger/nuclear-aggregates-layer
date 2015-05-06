using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowCards.Processors
{
    public class ImportFirmService : IImportFirmService
    {
        private readonly IImportFirmAggregateService _importFirmAggregateService;
        private readonly IIntegrationLocalizationSettings _integrationLocalizationSettings;
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        private readonly IUserContext _userContext;

        public ImportFirmService(IUserContext userContext,
                                 ISecurityServiceUserIdentifier securityServiceUserIdentifier,
                                 IIntegrationLocalizationSettings integrationLocalizationSettings,
                                 IMsCrmSettings msCrmSettings,
                                 IOperationScopeFactory scopeFactory,
                                 IImportFirmAggregateService importFirmAggregateService)
        {
            _userContext = userContext;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
            _integrationLocalizationSettings = integrationLocalizationSettings;
            _msCrmSettings = msCrmSettings;
            _scopeFactory = scopeFactory;
            _importFirmAggregateService = importFirmAggregateService;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var firmServiceBusDtos = dtos.Cast<FirmServiceBusDto>();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportFirmIdentity>())
            {
                var importFirmChanges = _importFirmAggregateService.ImportFirms(
                                                        firmServiceBusDtos,
                                                        _userContext.Identity.Code,
                                                        _securityServiceUserIdentifier.GetReserveUserIdentity().Code,
                                                        _integrationLocalizationSettings.RegionalTerritoryLocaleSpecificWord,
                                                        _msCrmSettings.IntegrationMode.HasFlag(MsCrmIntegrationMode.Database));

                scope.ApplyChanges<Firm>(importFirmChanges)
                     .ApplyChanges<FirmAddress>(importFirmChanges)
                     .Complete();
            }
        } 
    }
}