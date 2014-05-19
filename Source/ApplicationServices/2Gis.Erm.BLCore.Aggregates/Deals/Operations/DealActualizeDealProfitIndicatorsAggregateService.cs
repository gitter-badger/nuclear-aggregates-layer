using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.Operations
{
    public sealed class DealActualizeDealProfitIndicatorsAggregateService : IDealActualizeDealProfitIndicatorsAggregateService
    {
        private readonly IRepository<Deal> _dealRepository;
        private readonly ISecureRepository<Deal> _secureDealRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public DealActualizeDealProfitIndicatorsAggregateService(
            IRepository<Deal> dealRepository,
            ISecureRepository<Deal> secureDealRepository,
            IOperationScopeFactory scopeFactory)
        {
            _dealRepository = dealRepository;
            _secureDealRepository = secureDealRepository;
            _scopeFactory = scopeFactory;
        }

        public void Actualize(IEnumerable<DealActualizeProfitDto> dealInfos)
        {
            Actualize(dealInfos, deal => _dealRepository.Update(deal), () => _dealRepository.Save());
        }

        public void ActualizeSecure(IEnumerable<DealActualizeProfitDto> dealInfos)
        {
            Actualize(dealInfos, deal => _secureDealRepository.Update(deal), () => _secureDealRepository.Save());
        }

        private void Actualize(IEnumerable<DealActualizeProfitDto> dealInfos, Action<Deal> repositoryUpdate, Action repositorySave)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Deal>())
            {
                foreach (var info in dealInfos)
                {
                    info.Deal.EstimatedProfit = info.RemainingExpectedProfit;
                    info.Deal.ActualProfit = info.ActuallyReceivedProfit.HasValue ? info.ActuallyReceivedProfit.Value : info.Deal.ActualProfit;

                    repositoryUpdate(info.Deal);
                    scope.Updated<Deal>(info.Deal.Id);
                }

                repositorySave();
                scope.Complete();
            }
        }
    }
}