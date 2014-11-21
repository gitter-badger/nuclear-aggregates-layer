using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Georgaphy;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Territory;

namespace DoubleGis.Erm.BL.Operations.Concrete.Integration.Import
{
    public sealed class ImportSaleTerritoryService : IImportSaleTerritoryService
    {
        private readonly IImportSaleTerritoryAggregateService _importSaleTerritoryAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportSaleTerritoryService(IImportSaleTerritoryAggregateService importSaleTerritoryAggregateService, IOperationScopeFactory scopeFactory)
        {
            _importSaleTerritoryAggregateService = importSaleTerritoryAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var saleTerritoryServiceBusDtos = dtos.Cast<SaleTerritoryServiceBusDto>();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportSaleTerritoryIdentity>())
            {
                _importSaleTerritoryAggregateService.ImportTerritoryFromServiceBus(saleTerritoryServiceBusDtos);
                scope.Complete();
            }
        }
    }
}