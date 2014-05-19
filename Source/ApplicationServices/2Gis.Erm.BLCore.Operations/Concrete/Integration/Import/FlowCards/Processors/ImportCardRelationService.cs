using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowCards.Processors
{
    public class ImportCardRelationService : IImportCardRelationService
    {
        private readonly IImportCardRelationAggregateService _importCardRelationAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportCardRelationService(IImportCardRelationAggregateService importCardRelationAggregateService, IOperationScopeFactory scopeFactory)
        {
            _importCardRelationAggregateService = importCardRelationAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var cardRelationServiceBusDtos = dtos.Cast<CardRelationServiceBusDto>();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportCardRelationIdentity>())
            {
                _importCardRelationAggregateService.ImportCardRelations(cardRelationServiceBusDtos);
                scope.Complete();
            }
        }
    }
}