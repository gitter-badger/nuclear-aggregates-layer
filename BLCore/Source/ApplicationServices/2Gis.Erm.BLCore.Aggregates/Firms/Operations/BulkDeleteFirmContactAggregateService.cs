using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkDeleteFirmContactAggregateService : IBulkDeleteFirmContactAggregateService
    {
        private readonly IRepository<FirmContact> _firmContactRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkDeleteFirmContactAggregateService(IRepository<FirmContact> firmContactRepository, IOperationScopeFactory operationScopeFactory)
        {
            _firmContactRepository = firmContactRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Delete(IReadOnlyCollection<FirmContact> firmContacts)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<BulkDeleteIdentity, FirmContact>())
            {
                _firmContactRepository.DeleteRange(firmContacts);
                _firmContactRepository.Save();

                scope.Deleted(firmContacts.AsEnumerable())
                     .Complete();
            }
        }
    }
}