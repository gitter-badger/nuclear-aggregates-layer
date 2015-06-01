using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkUpdateFirmAggregateService : IBulkUpdateFirmAggregateService
    {
        private readonly IRepository<Firm> _firmRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkUpdateFirmAggregateService(IRepository<Firm> firmRepository, IOperationScopeFactory operationScopeFactory)
        {
            _firmRepository = firmRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Update(IReadOnlyCollection<Firm> firms)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<BulkUpdateIdentity, Firm>())
            {
                foreach (var firm in firms)
                {
                    _firmRepository.Update(firm);
                    scope.Updated(firm);
                }

                _firmRepository.Save();
                scope.Complete();
            }
        }
    }
}