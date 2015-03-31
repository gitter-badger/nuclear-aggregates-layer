using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public sealed class ImportFirmAggregateService : IImportFirmAggregateService
    {
        // timeout should be increased due to long sql updates
        private readonly TimeSpan _importCommandTimeout = TimeSpan.FromMinutes(15);

        private readonly IFirmPersistenceService _firmPersistenceService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _tracer;

        public ImportFirmAggregateService(
            IFirmPersistenceService firmPersistenceService, 
            IOperationScopeFactory scopeFactory,
            ITracer tracer)
        {
            _firmPersistenceService = firmPersistenceService;
            _scopeFactory = scopeFactory;
            _tracer = tracer;
        }

        public EntityChangesContext ImportFirms(IEnumerable<FirmServiceBusDto> dtos,
                                             long userId,
                                             long reserveUserId,
                                             string regionalTerritoryLocaleSpecificWord,
                                             bool enableReplication)
        {
            var firmsXml = string.Format("<Root>{0}</Root>", string.Concat(dtos.Select(x => x.Content.ToString())));

            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Firm>())
            {
                var importFirmChanges = _firmPersistenceService.ImportFirmFromXml(
                                                                    firmsXml,
                                                                    userId,
                                                                    reserveUserId,
                                                                    _importCommandTimeout,
                                                                    enableReplication,
                                                                    regionalTerritoryLocaleSpecificWord);

                long[] firmIdsFromParsedDto = GetFirmIdsFromParsedDto(dtos);
                IReadOnlyDictionary<ChangesType, IReadOnlyDictionary<long, int>> mergedImportFirmChanges;

                scope.Updated<Firm>(firmIdsFromParsedDto)
                     .ApplyChanges<Firm>(importFirmChanges, out mergedImportFirmChanges)
                     .ApplyChanges<FirmAddress>(importFirmChanges)
                     .Complete();

                // TODO {all, 21.05.2014}: защитный код - если расхождений между списками фирм из DTO и от AggregateService не будет, то можно выпилить, также как firmIdsFromParsedDto
                var diffParsedDtoAndAggregateService = firmIdsFromParsedDto.Except(
                                                              mergedImportFirmChanges[ChangesType.Added].Keys
                                                              .Union(mergedImportFirmChanges[ChangesType.Updated].Keys)
                                                              .Union(mergedImportFirmChanges[ChangesType.Deleted].Keys))
                                                              .ToArray();
                if (diffParsedDtoAndAggregateService.Any())
                {
                    importFirmChanges.Updated<Firm>(diffParsedDtoAndAggregateService);
                    _tracer.Warn("There are different firm changes detected from parsed dto and reported by aggregate service. Divergent firm ids count: " + diffParsedDtoAndAggregateService.Length);
                    _tracer.Debug("Divergent firm ids (DiffParsedDtoAndAggregateService): " + string.Join(",", diffParsedDtoAndAggregateService));
                }

                return importFirmChanges;
            }
        }

        private long[] GetFirmIdsFromParsedDto(IEnumerable<FirmServiceBusDto> dtos)
        {
            return dtos.Where(x => x.Content.Name == "Firm")
                       .Select(x => (long)x.Content.Attribute("Code"))
                       .ToArray();
        }
    }
}