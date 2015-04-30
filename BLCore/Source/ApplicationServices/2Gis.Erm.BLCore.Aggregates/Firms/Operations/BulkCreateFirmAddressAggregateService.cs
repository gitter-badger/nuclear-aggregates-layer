using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkCreateFirmAddressAggregateService : IBulkCreateFirmAddressAggregateService
    {
        private readonly IRepository<FirmAddress> _firmAddressRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkCreateFirmAddressAggregateService(IRepository<FirmAddress> firmAddressRepository, IOperationScopeFactory operationScopeFactory)
        {
            _firmAddressRepository = firmAddressRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Create(IReadOnlyCollection<FirmAddress> firmAddresses)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<BulkCreateIdentity, FirmAddress>())
            {
                _firmAddressRepository.AddRange(firmAddresses);
                _firmAddressRepository.Save();

                scope.Added(firmAddresses.AsEnumerable())
                     .Complete();
            }
        }
    }
}