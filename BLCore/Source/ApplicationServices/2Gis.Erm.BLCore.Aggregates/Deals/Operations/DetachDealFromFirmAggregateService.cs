using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.Operations
{
    public class DetachDealFromFirmAggregateService : IDetachDealFromFirmAggregateService
    {
        private readonly IRepository<Deal> _dealRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public DetachDealFromFirmAggregateService(IRepository<Deal> dealRepository, IOperationScopeFactory scopeFactory)
        {
            _dealRepository = dealRepository;
            _scopeFactory = scopeFactory;
        }

        public void Detach(IEnumerable<Deal> dealsToDetach)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DetachIdentity, Deal, Firm>())
            {
                foreach (var deal in dealsToDetach)
                {
                    deal.MainFirmId = null;
                    _dealRepository.Update(deal);
                    scope.Updated(deal);
                }

                _dealRepository.Save();
                scope.Complete();
            }
        }
    }
}