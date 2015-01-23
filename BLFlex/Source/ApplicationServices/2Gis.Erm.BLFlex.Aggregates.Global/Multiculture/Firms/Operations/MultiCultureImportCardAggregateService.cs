using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.BLFlex.API.Aggregates.Global.MultiCulture.Firms.Operations;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Firms.Operations
{
    public class MultiCultureImportCardAggregateService : IImportCardAggregateService, IRussiaAdapted, IChileAdapted, ICyprusAdapted, ICzechAdapted,
                                                          IUkraineAdapted, IKazakhstanAdapted
    {
        // timeout should be increased due to long sql updates
        private readonly TimeSpan _importCommandTimeout = TimeSpan.FromMinutes(15);

        private readonly IFirmPersistenceService _firmPersistenceService;
        private readonly IOperationScopeFactory _scopeFactory;

        public MultiCultureImportCardAggregateService(IFirmPersistenceService firmPersistenceService, IOperationScopeFactory scopeFactory)
        {
            _firmPersistenceService = firmPersistenceService;
            _scopeFactory = scopeFactory;
        }

        public EntityChangesContext ImportCards(IEnumerable<MultiCultureCardServiceBusDto> dtos,
                                             long userId,
                                             long reserveUserId,
                                             long[] pregeneratedIds,
                                             string regionalTerritoryLocaleSpecificWord,
                                             bool enableReplication)
        {
            var cardsXml = string.Format("<Root>{0}</Root>", string.Concat(dtos.Select(x => x.Content.ToString())));

            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Firm>())
            {
                var changesContext = _firmPersistenceService.ImportCardsFromXml(cardsXml,
                                                                            userId,
                                                                            reserveUserId,
                                                                            _importCommandTimeout,
                                                                            pregeneratedIds,
                                                                            regionalTerritoryLocaleSpecificWord);

                scope.ApplyChanges<Firm>(changesContext)
                     .ApplyChanges<FirmAddress>(changesContext)
                     .Complete();

                return changesContext;
            }
        }
    }
}