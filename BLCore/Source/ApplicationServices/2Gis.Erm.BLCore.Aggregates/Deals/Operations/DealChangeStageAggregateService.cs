using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.Operations
{
    public sealed class DealChangeStageAggregateService : IDealChangeStageAggregateService
    {
        private readonly IRepository<Deal> _dealRepository;
        private readonly ISecureRepository<Deal> _secureDealRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public DealChangeStageAggregateService(
            IRepository<Deal> dealRepository,
            ISecureRepository<Deal> secureDealRepository,
            IOperationScopeFactory scopeFactory)
        {
            _dealRepository = dealRepository;
            _secureDealRepository = secureDealRepository;
            _scopeFactory = scopeFactory;
        }

        public IEnumerable<ChangesDescriptor> ChangeStage(IEnumerable<DealChangeStageDto> dealInfos)
        {
            return ChangeStage(dealInfos, deal => _dealRepository.Update(deal), () => _dealRepository.Save());
        }

        public IEnumerable<ChangesDescriptor> ChangeStageSecure(IEnumerable<DealChangeStageDto> dealInfos)
        {
            return ChangeStage(dealInfos, deal => _secureDealRepository.Update(deal), () => _secureDealRepository.Save());
        }

        private IEnumerable<ChangesDescriptor> ChangeStage(IEnumerable<DealChangeStageDto> dealInfos, Action<Deal> repositoryUpdate, Action repositorySave)
        {
            var changes = new List<ChangesDescriptor>();

            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Deal>())
            {
                foreach (var info in dealInfos)
                {
                    changes.Add(ChangesDescriptor.Create(info.Deal, d => d.DealStage, info.Deal.DealStage, info.NextStage));

                    info.Deal.DealStage = info.NextStage;
                    repositoryUpdate(info.Deal);
                    scope.Updated<Deal>(info.Deal.Id);
                }

                repositorySave();
                scope.Complete();
            }

            return changes;
        }
    }
}