using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkUpdateFirmAddressAggregateService : IBulkUpdateFirmAddressAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<FirmAddress> _firmAddressRepository;

        public BulkUpdateFirmAddressAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<FirmAddress> firmAddressRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _firmAddressRepository = firmAddressRepository;
        }

        public void Update(IReadOnlyCollection<FirmAddress> firmAddresses)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, FirmAddress>())
            {
                foreach (var firmAddress in firmAddresses)
                {
                    _firmAddressRepository.Update(firmAddress);
                    scope.Updated(firmAddress);
                }

                _firmAddressRepository.Save();
                scope.Complete();
            }
        }
    }
}