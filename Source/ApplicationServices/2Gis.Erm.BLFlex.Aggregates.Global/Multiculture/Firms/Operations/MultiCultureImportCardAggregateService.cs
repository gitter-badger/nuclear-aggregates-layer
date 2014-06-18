using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.BLFlex.API.Aggregates.Global.MultiCulture.Firms.Operations;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Firms.Operations
{
    public class MultiCultureImportCardAggregateService : IImportCardAggregateService, IRussiaAdapted, IChileAdapted, ICyprusAdapted, ICzechAdapted,
                                                          IUkraineAdapted
    {
        // timeout should be increased due to long sql updates (15:00:00 min = 900 sec)
        private const int ImportCommandTimeout = 900;

        private readonly IFirmPersistenceService _firmPersistanceService;

        public MultiCultureImportCardAggregateService(IFirmPersistenceService firmPersistanceService)
        {
            _firmPersistanceService = firmPersistanceService;
        }

        public IEnumerable<long> ImportCards(IEnumerable<MultiCultureCardServiceBusDto> dtos,
                                             long userId,
                                             long reserveUserId,
                                             long[] pregeneratedIds,
                                             string regionalTerritoryLocaleSpecificWord,
                                             bool enableReplication)
        {
            var cardsXml = string.Format("<Root>{0}</Root>", string.Concat(dtos.Select(x => x.Content.ToString())));

            var updatedIds = _firmPersistanceService.ImportCardsFromXml(cardsXml,
                                                                        userId,
                                                                        reserveUserId,
                                                                        ImportCommandTimeout,
                                                                        pregeneratedIds,
                                                                        regionalTerritoryLocaleSpecificWord);

            return updatedIds;
        }
    }
}