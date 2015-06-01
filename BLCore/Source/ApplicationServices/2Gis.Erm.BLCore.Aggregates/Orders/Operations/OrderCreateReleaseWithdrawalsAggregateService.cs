using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel.DTO;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public sealed class OrderCreateReleaseWithdrawalsAggregateService : IOrderCreateReleaseWithdrawalsAggregateService
    {
        private readonly IRepository<ReleaseWithdrawal> _releaseWithdrawalsRepository;
        private readonly IRepository<ReleasesWithdrawalsPosition> _releaseWithdrawalsPositionsRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public OrderCreateReleaseWithdrawalsAggregateService(
            IRepository<ReleaseWithdrawal> releaseWithdrawalsRepository,
            IRepository<ReleasesWithdrawalsPosition> releaseWithdrawalsPositionsRepository,
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _releaseWithdrawalsRepository = releaseWithdrawalsRepository;
            _releaseWithdrawalsPositionsRepository = releaseWithdrawalsPositionsRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void Create(IEnumerable<OrderReleaseWithdrawalDto> releaseWithdrawals)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, ReleaseWithdrawal>())
            {
                foreach (var releaseWithdrawal in releaseWithdrawals)
                {
                    _identityProvider.SetFor(releaseWithdrawal.WidrawalInfo);
                    _releaseWithdrawalsRepository.Add(releaseWithdrawal.WidrawalInfo);
                    scope.Added<ReleaseWithdrawal>(releaseWithdrawal.WidrawalInfo.Id);

                    foreach (var releasesWithdrawalsPosition in releaseWithdrawal.WithdrawalsPositions)
                    {
                        releasesWithdrawalsPosition.ReleasesWithdrawalId = releaseWithdrawal.WidrawalInfo.Id;
                        _identityProvider.SetFor(releasesWithdrawalsPosition);
                        _releaseWithdrawalsPositionsRepository.Add(releasesWithdrawalsPosition);
                        scope.Added<ReleasesWithdrawalsPosition>(releasesWithdrawalsPosition.Id);
                    }
                }

                _releaseWithdrawalsRepository.Save();
                _releaseWithdrawalsPositionsRepository.Save();
                
                scope.Complete();
            }
        }
    }
}