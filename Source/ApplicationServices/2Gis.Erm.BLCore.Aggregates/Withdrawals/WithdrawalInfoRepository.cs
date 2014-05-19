using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Withdrawals
{
    public class WithdrawalInfoRepository : IWithdrawalInfoRepository
    {
        private readonly IFinder _finder;
        private readonly IRepository<ReleaseWithdrawal> _releaseWithdrawalGenericRepository;
        private readonly IRepository<WithdrawalInfo> _withdrawalInfoGenericRepository;
        private readonly IRepository<ReleasesWithdrawalsPosition> _releaseWithdrawalPositionGenericRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public WithdrawalInfoRepository(IFinder finder,
                                        IRepository<WithdrawalInfo> withdrawalInfoGenericRepository,
                                        IRepository<ReleaseWithdrawal> releaseWithdrawalGenericRepository,
                                        IRepository<ReleasesWithdrawalsPosition> releaseWithdrawalPositionGenericRepository,
                                        IIdentityProvider identityProvider,
                                        IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _withdrawalInfoGenericRepository = withdrawalInfoGenericRepository;
            _releaseWithdrawalGenericRepository = releaseWithdrawalGenericRepository;
            _releaseWithdrawalPositionGenericRepository = releaseWithdrawalPositionGenericRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public int Create(IEnumerable<ReleasesWithdrawalsPosition> releaseWithdrawalPositions)
        {
            int cnt = 0;
            foreach (var releaseWithdrawalPosition in releaseWithdrawalPositions)
            {
                // TODO {all, 09.09.2013}: В процессе рефакторинга перевести на операцию с BulkCreateIdentity
                using (var scope = _scopeFactory.CreateOrUpdateOperationFor(releaseWithdrawalPosition))
                {
                    _identityProvider.SetFor(releaseWithdrawalPosition);
                    _releaseWithdrawalPositionGenericRepository.Add(releaseWithdrawalPosition);
                    cnt += _releaseWithdrawalPositionGenericRepository.Save();

                    scope.Added<ReleasesWithdrawalsPosition>(releaseWithdrawalPosition.Id)
                         .Complete();
                }
            }

            return cnt;
        }

        public int Create(IEnumerable<ReleaseWithdrawal> releaseWithdrawals)
        {
            int cnt = 0;
            foreach (var releaseWithdrawal in releaseWithdrawals)
            {
                // TODO {all, 09.09.2013}: В процессе рефакторинга перевести на операцию с BulkCreateIdentity
                using (var scope = _scopeFactory.CreateOrUpdateOperationFor(releaseWithdrawal))
                {
                    _identityProvider.SetFor(releaseWithdrawal);
                    _releaseWithdrawalGenericRepository.Add(releaseWithdrawal);
                    cnt += _releaseWithdrawalGenericRepository.Save();

                    scope.Added<ReleaseWithdrawal>(releaseWithdrawal.Id)
                         .Complete();
                }
            }

            return cnt;
        }

        public int CreateOrUpdate(WithdrawalInfo withdrawal)
        {
            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(withdrawal))
            {
                if (withdrawal.IsNew())
                {
                    _identityProvider.SetFor(withdrawal);
                    _withdrawalInfoGenericRepository.Add(withdrawal);

                    scope.Added<WithdrawalInfo>(withdrawal.Id);
                }
                else
                {
                    _withdrawalInfoGenericRepository.Update(withdrawal);

                    scope.Updated<WithdrawalInfo>(withdrawal.Id);
                }

                var cnt = _withdrawalInfoGenericRepository.Save();
                scope.Complete();

                return cnt;
            }
        }

        public int Delete(IEnumerable<ReleaseWithdrawal> releaseWithdrawals)
        {
            int cnt = 0;
            foreach (var withdrawal in releaseWithdrawals)
            {
                // TODO {all, 09.09.2013}: В процессе рефакторинга перевести на операцию с BulkDeleteIdentity
                using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, ReleaseWithdrawal>())
                {
                    _releaseWithdrawalGenericRepository.Delete(withdrawal);
                    cnt += _releaseWithdrawalGenericRepository.Save();

                    scope.Deleted<ReleaseWithdrawal>(withdrawal.Id)
                         .Complete();
                }
            }

            return cnt;
        }

        public decimal? TakeAmountToWithdrawForOrder(long orderId, int skip, int take)
        {
            return _finder.Find<OrderReleaseTotal>(x => x.OrderId == orderId)
                .OrderBy(x => x.Id)
                .Skip(skip)
                .Take(take)
                .Select(x => (decimal?)x.AmountToWithdraw)
                .SingleOrDefault();
        }

        public long[] DeleteReleaseWithdrawalPositionsForOrder(long orderId)
        {
            var releaseWithdrawalPositions = _finder.Find<ReleaseWithdrawal>(x => x.OrderPosition.OrderId == orderId)
                                                    .SelectMany(x => x.ReleasesWithdrawalsPositions)
                                                    .ToArray();
            
            foreach (var withdrawalPosition in releaseWithdrawalPositions)
            {
                // TODO {all, 09.09.2013}: В процессе рефакторинга перевести на операцию с BulkDeleteIdentity
                using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, ReleasesWithdrawalsPosition>())
                {
                    _releaseWithdrawalPositionGenericRepository.Delete(withdrawalPosition);
                    _releaseWithdrawalPositionGenericRepository.Save();

                    scope.Deleted<ReleasesWithdrawalsPosition>(withdrawalPosition.Id)
                         .Complete();
                }
            }

            return releaseWithdrawalPositions.Select(position => position.Id).ToArray();
        }

        public long[] DeleteReleaseWithdrawalsForOrder(long orderId)
        {
            var releaseWithdrawals = _finder.Find<ReleaseWithdrawal>(x => x.OrderPosition.OrderId == orderId)
                                            .ToArray();

            foreach (var withdrawal in releaseWithdrawals)
            {
                // TODO {all, 09.09.2013}: В процессе рефакторинга перевести на операцию с BulkDeleteIdentity
                using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, ReleaseWithdrawal>())
                {
                    _releaseWithdrawalGenericRepository.Delete(withdrawal);
                    _releaseWithdrawalGenericRepository.Save();

                    scope.Deleted<ReleaseWithdrawal>(withdrawal.Id)
                         .Complete();
                }
            }

            return releaseWithdrawals.Select(withdrawal => withdrawal.Id).ToArray();
        }
    }
}
