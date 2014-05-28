using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Withdrawals.Operations
{
    public class BulkDeleteChargesAggregateService : IBulkDeleteChargesAggregateService
    {
        private readonly IRepository<Charge> _chargeRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public BulkDeleteChargesAggregateService(IRepository<Charge> chargeRepository, IOperationScopeFactory scopeFactory)
        {
            _chargeRepository = chargeRepository;
            _scopeFactory = scopeFactory;
        }

        public void Delete(IReadOnlyCollection<Charge> chargesToDelete)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<BulkDeleteIdentity, Charge>())
            {
                _chargeRepository.DeleteRange(chargesToDelete);
                _chargeRepository.Save();

                scope.Deleted<Charge>(chargesToDelete.Select(x => x.Id)).Complete();
            }
        }
    }
}