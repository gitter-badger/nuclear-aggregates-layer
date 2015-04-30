using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkUpdateFirmContactAggregateService : IBulkUpdateFirmContactAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<FirmContact> _firmContactRepository;

        public BulkUpdateFirmContactAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<FirmContact> firmContactRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _firmContactRepository = firmContactRepository;
        }

        public void Update(IReadOnlyCollection<FirmContact> firmContacts)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, FirmContact>())
            {
                foreach (var firmContact in firmContacts)
                {
                    _firmContactRepository.Update(firmContact);
                    operationScope.Updated(firmContact);
                }

                _firmContactRepository.Save();
                operationScope.Complete();
            }
        }
    }
}