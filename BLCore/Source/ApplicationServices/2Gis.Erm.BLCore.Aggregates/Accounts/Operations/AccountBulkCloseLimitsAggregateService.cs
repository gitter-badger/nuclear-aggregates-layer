using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public sealed class AccountBulkCloseLimitsAggregateService : IAccountBulkCloseLimitsAggregateService
    {
        private readonly IRepository<Limit> _limitRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _logger;

        public AccountBulkCloseLimitsAggregateService(
            IRepository<Limit> limitRepository,
            IOperationScopeFactory scopeFactory,
            ITracer logger)
        {
            _limitRepository = limitRepository;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void Close(IEnumerable<Limit> limits)
        {
            int closedLimits = 0;
            using (var scope = _scopeFactory.CreateSpecificFor<BulkDeactivateIdentity, Limit>())
            {
                var closeDate = DateTime.UtcNow;

                foreach (var limit in limits)
                {
                    limit.IsActive = false;
                    limit.CloseDate = closeDate;

                    _limitRepository.Update(limit);
                    scope.Updated<Limit>(limit.Id);

                    ++closedLimits;
                }

                _limitRepository.Save();
                scope.Complete();
            }

            _logger.Info("Closed limits count: " + closedLimits);
        }
    }
}