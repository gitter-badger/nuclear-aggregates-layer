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
    public class ImportReferenceService : IImportReferenceService
    {
        private readonly IImportReferenceAggregateService _importReferenceAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportReferenceService(IImportReferenceAggregateService importReferenceAggregateService, IOperationScopeFactory scopeFactory)
        {
            _importReferenceAggregateService = importReferenceAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var referenceServiceBusDtos = dtos.Cast<ReferenceServiceBusDto>().ToArray();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportReferenceIdentity>())
            {
                _importReferenceAggregateService.ImportReferences(referenceServiceBusDtos);
                scope.Complete();
            }
        }
    }
}