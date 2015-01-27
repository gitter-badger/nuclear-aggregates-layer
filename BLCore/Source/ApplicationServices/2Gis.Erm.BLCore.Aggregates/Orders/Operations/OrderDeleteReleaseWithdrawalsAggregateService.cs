using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel.DTO;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public sealed class OrderDeleteReleaseWithdrawalsAggregateService : IOrderDeleteReleaseWithdrawalsAggregateService
    {
        private readonly IRepository<ReleaseWithdrawal> _releaseWithdrawalsRepository;
        private readonly IRepository<ReleasesWithdrawalsPosition> _releaseWithdrawalsPositionsRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public OrderDeleteReleaseWithdrawalsAggregateService(
            IRepository<ReleaseWithdrawal> releaseWithdrawalsRepository,
            IRepository<ReleasesWithdrawalsPosition> releaseWithdrawalsPositionsRepository, 
            IOperationScopeFactory scopeFactory)
        {
            _releaseWithdrawalsRepository = releaseWithdrawalsRepository;
            _releaseWithdrawalsPositionsRepository = releaseWithdrawalsPositionsRepository;
            _scopeFactory = scopeFactory;
        }

        public void Delete(IEnumerable<OrderReleaseWithdrawalDto> releaseWithdrawals)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, ReleaseWithdrawal>())
            {
                foreach (var releaseWithdrawal in releaseWithdrawals)
                {
                    _releaseWithdrawalsRepository.Delete(releaseWithdrawal.WidrawalInfo);
                    scope.Deleted<ReleaseWithdrawal>(releaseWithdrawal.WidrawalInfo.Id);

                    foreach (var releasesWithdrawalsPosition in releaseWithdrawal.WithdrawalsPositions)
                    {
                        _releaseWithdrawalsPositionsRepository.Delete(releasesWithdrawalsPosition);
                        scope.Deleted<ReleasesWithdrawalsPosition>(releasesWithdrawalsPosition.Id);
                    }
                }

                _releaseWithdrawalsPositionsRepository.Save();
                _releaseWithdrawalsRepository.Save();
                scope.Complete();
            }
        }
    }
}