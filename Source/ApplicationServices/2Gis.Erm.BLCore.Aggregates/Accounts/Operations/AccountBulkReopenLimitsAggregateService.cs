using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public sealed class AccountBulkReopenLimitsAggregateService : IAccountBulkReopenLimitsAggregateService
    {
        private readonly IRepository<Limit> _limitRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public AccountBulkReopenLimitsAggregateService(
            IRepository<Limit> limitRepository,
            IOperationScopeFactory scopeFactory,
            ICommonLog logger)
        {
            _limitRepository = limitRepository;
            _scopeFactory = scopeFactory;
            _logger = logger;
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

            _logger.InfoEx("Reopened limits count: " + reopenedLimits);
        }
    }
}