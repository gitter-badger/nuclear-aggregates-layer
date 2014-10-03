﻿using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.Operations
{
    public class CreateFirmDealAggregateService : ICreateFirmDealAggregateService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<FirmDeal> _firmDealLinkRepository;

        public CreateFirmDealAggregateService(
            IIdentityProvider identityProvider,
            IOperationScopeFactory operationScopeFactory,
            IRepository<FirmDeal> firmDealLinkRepository)
        {
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
            _firmDealLinkRepository = firmDealLinkRepository;
        }

        public void Create(FirmDeal entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, FirmDeal>())
            {
                _identityProvider.SetFor(entity);
                _firmDealLinkRepository.Add(entity);
                operationScope.Added(entity);

                _firmDealLinkRepository.Save();

                operationScope.Complete();
            }
        }
    }
}