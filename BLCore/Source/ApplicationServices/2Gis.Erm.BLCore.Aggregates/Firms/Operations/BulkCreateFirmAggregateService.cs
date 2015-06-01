using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkCreateFirmAggregateService : IBulkCreateFirmAggregateService
    {
        private readonly IRepository<Firm> _firmRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkCreateFirmAggregateService(IRepository<Firm> firmRepository, IOperationScopeFactory operationScopeFactory)
        {
            _firmRepository = firmRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Create(IReadOnlyCollection<Firm> firms)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<BulkCreateIdentity, Firm>())
            {
                _firmRepository.AddRange(firms);
                _firmRepository.Save();

                operationScope.Added(firms.AsEnumerable())
                              .Complete();
            }
        }
    }
}