using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public sealed class AccountBulkReopenLimitsAggregateService : IAccountBulkReopenLimitsAggregateService
    {
        private readonly IRepository<Limit> _limitRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _tracer;

        public AccountBulkReopenLimitsAggregateService(
            IRepository<Limit> limitRepository,
            IOperationScopeFactory scopeFactory,
            ITracer tracer)
        {
            _limitRepository = limitRepository;
            _scopeFactory = scopeFactory;
            _tracer = tracer;
        }

        public void Reopen(IEnumerable<Limit> limits)
        {
            int reopenedLimits = 0;
            using (var scope = _scopeFactory.CreateSpecificFor<BulkDeactivateIdentity, Limit>())
            {
                foreach (var limit in limits)
                {
                    limit.IsActive = true;
                    limit.CloseDate = null;

                    _limitRepository.Update(limit);
                    scope.Updated<Limit>(limit.Id);

                    ++reopenedLimits;
                }

                _limitRepository.Save();
                scope.Complete();
            }

            _tracer.Info("Reopened limits count: " + reopenedLimits);
        }
    }
}