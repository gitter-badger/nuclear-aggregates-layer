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
    public class ImportReferenceItemService : IImportReferenceItemService
    {
        private readonly IImportReferenceItemAggregateService _importReferenceItemAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportReferenceItemService(IImportReferenceItemAggregateService importReferenceItemAggregateService, IOperationScopeFactory scopeFactory)
        {
            _importReferenceItemAggregateService = importReferenceItemAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var referenceItemServiceBusDtos = dtos.Cast<ReferenceItemServiceBusDto>();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportReferenceItemIdentity>())
            {
                _importReferenceItemAggregateService.ImportReferenceItems(referenceItemServiceBusDtos);
                scope.Complete();
            }
        }
    }
}