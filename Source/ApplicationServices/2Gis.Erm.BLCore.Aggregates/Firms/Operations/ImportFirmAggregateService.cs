using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class ImportFirmAggregateService : IImportFirmAggregateService
    {
        // timeout should be increased due to long sql updates (15:00:00 min = 900 sec)
        private const int ImportCommandTimeout = 900;

        private readonly IFirmPersistenceService _firmPersistenceService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportFirmAggregateService(IFirmPersistenceService firmPersistenceService, IOperationScopeFactory scopeFactory)
        {
            _firmPersistenceService = firmPersistenceService;
            _scopeFactory = scopeFactory;
        }

        public IEnumerable<long> ImportFirms(IEnumerable<FirmServiceBusDto> dtos,
                                             long userId,
                                             long reserveUserId,
                                             string regionalTerritoryLocaleSpecificWord,
                                             bool enableReplication)
        {
            var firmsXml = string.Format("<Root>{0}</Root>", string.Concat(dtos.Select(x => x.Content.ToString())));

            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Firm>())
            {
                _firmPersistenceService.ImportFirmFromXml(firmsXml,
                                                          userId,
                                                          reserveUserId,
                                                          ImportCommandTimeout,
                                                          enableReplication,
                                                          regionalTerritoryLocaleSpecificWord);

                var firmIds = GetFirmIdsFromParsedDto(dtos);
                scope.Updated<Firm>(firmIds)
                     .Complete();

                return firmIds;
            }
        }

        private IEnumerable<long> GetFirmIdsFromParsedDto(IEnumerable<FirmServiceBusDto> dtos)
        {
            return dtos.Where(x => x.Content.Name == "Firm")
                       .Select(x => (long)x.Content.Attribute("Code"))
                       .ToArray();
        }
    }
}