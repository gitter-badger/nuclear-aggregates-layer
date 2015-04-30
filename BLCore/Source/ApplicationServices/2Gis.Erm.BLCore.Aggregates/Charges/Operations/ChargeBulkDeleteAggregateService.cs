using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Charges.Operations
{
    public sealed class ChargeBulkDeleteAggregateService : IChargeBulkDeleteAggregateService
    {
        private readonly IRepository<Charge> _chargeRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public ChargeBulkDeleteAggregateService(IRepository<Charge> chargeRepository, IOperationScopeFactory scopeFactory)
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