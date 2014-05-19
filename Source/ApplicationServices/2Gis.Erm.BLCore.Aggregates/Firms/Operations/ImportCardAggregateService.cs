using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class ImportCardAggregateService : IImportCardAggregateService
    {
        // timeout should be increased due to long sql updates (15:00:00 min = 900 sec)
        private const int ImportCommandTimeout = 900;

        private readonly IFirmPersistenceService _firmPersistanceService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportCardAggregateService(IOperationScopeFactory scopeFactory, IFirmPersistenceService firmPersistanceService)
        {
            _scopeFactory = scopeFactory;
            _firmPersistanceService = firmPersistanceService;
        }

        public IEnumerable<long> ImportCards(IEnumerable<CardServiceBusDto> dtos,
                                             long userId,
                                             long reserveUserId,
                                             long[] pregeneratedIds,
                                             string regionalTerritoryLocaleSpecificWord,
                                             bool enableReplication)
        {
            var cardsXml = string.Format("<Root>{0}</Root>", string.Concat(dtos.Select(x => x.Content.ToString())));

            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Firm>())
            {
                var updatedIds = _firmPersistanceService.ImportCardsFromXml(cardsXml,
                                                                            userId,
                                                                            reserveUserId,
                                                                            ImportCommandTimeout,
                                                                            pregeneratedIds,
                                                                            regionalTerritoryLocaleSpecificWord);

                scope.Updated<Firm>(updatedIds)
                     .Complete();

                return updatedIds;
            }
        }
    }
}