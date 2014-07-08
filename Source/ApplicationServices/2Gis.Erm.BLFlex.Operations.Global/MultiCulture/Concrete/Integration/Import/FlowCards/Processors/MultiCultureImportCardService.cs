﻿using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLFlex.API.Aggregates.Global.MultiCulture.Firms.Operations;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Integration.Import.FlowCards.Processors
{
    public class MultiCultureImportCardService : IImportCardService, IRussiaAdapted, IChileAdapted, ICyprusAdapted, ICzechAdapted, IUkraineAdapted
    {
        private const int PregeneratedIdsAmount = 3000;
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IImportCardAggregateService _importCardAggregateService;

        private readonly IIntegrationLocalizationSettings _integrationLocalizationSettings;
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        private readonly IUserContext _userContext;

        public MultiCultureImportCardService(IUserContext userContext,
                                 ISecurityServiceUserIdentifier securityServiceUserIdentifier,
                                 IIntegrationLocalizationSettings integrationLocalizationSettings,
                                 IMsCrmSettings msCrmSettings,
                                 IClientProxyFactory clientProxyFactory,
                                 IOperationScopeFactory scopeFactory,
                                 IImportCardAggregateService importCardAggregateService)
        {
            _userContext = userContext;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
            _integrationLocalizationSettings = integrationLocalizationSettings;
            _msCrmSettings = msCrmSettings;
            _clientProxyFactory = clientProxyFactory;
            _scopeFactory = scopeFactory;
            _importCardAggregateService = importCardAggregateService;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var cardServiceBusDtos = dtos.Cast<MultiCultureCardServiceBusDto>();

            var ids = _clientProxyFactory.GetClientProxy<IIdentityProviderApplicationService, WSHttpBinding>()
                                         .Execute(x => x.GetIdentities(PregeneratedIdsAmount));

            using (var scope = _scopeFactory.CreateNonCoupled<ImportCardIdentity>())
            {
                var changesContext = _importCardAggregateService.ImportCards(cardServiceBusDtos,
                                                                      _userContext.Identity.Code,
                                                                      _securityServiceUserIdentifier.GetReserveUserIdentity().Code,
                                                                      ids,
                                                                      _integrationLocalizationSettings.RegionalTerritoryLocaleSpecificWord,
                                                                      _msCrmSettings.EnableReplication);

                scope.ApplyChanges<Firm>(changesContext)
                     .ApplyChanges<FirmAddress>(changesContext)
                     .Complete();
            }
        }
    }
}