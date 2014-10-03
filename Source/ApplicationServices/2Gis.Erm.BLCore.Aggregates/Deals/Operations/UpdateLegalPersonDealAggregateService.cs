﻿using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.Operations
{
    public class UpdateLegalPersonDealAggregateService : IUpdateLegalPersonDealAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<LegalPersonDeal> _legalPersonDealLinkRepository;

        public UpdateLegalPersonDealAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<LegalPersonDeal> legalPersonDealLinkRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _legalPersonDealLinkRepository = legalPersonDealLinkRepository;
        }

        public void Update(LegalPersonDeal legalPersonDeal)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, LegalPersonDeal>())
            {
                _legalPersonDealLinkRepository.Update(legalPersonDeal);
                operationScope.Updated(legalPersonDeal);

                _legalPersonDealLinkRepository.Save();

                operationScope.Complete();
            }
        }
    }
}