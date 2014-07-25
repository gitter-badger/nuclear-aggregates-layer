﻿using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Bargains
{
    public class ActivateBargainAggregateService : IAggregateRootRepository<Order>, IActivateAggregateRepository<Bargain>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<Bargain> _entityRepository;
        private readonly ISecureFinder _finder;

        public ActivateBargainAggregateService(IOperationScopeFactory operationScopeFactory,
                                               ISecureRepository<Bargain> entityRepository,
                                               ISecureFinder finder)
        {
            _operationScopeFactory = operationScopeFactory;
            _entityRepository = entityRepository;
            _finder = finder;
        }

        public int Activate(Bargain bargain)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, Bargain>())
            {
                bargain.IsActive = true;
                _entityRepository.Update(bargain);
                operationScope.Updated<Bargain>(bargain.Id);

                var count = _entityRepository.Save();

                operationScope.Complete();

                return count;
            }
        }

        public int Activate(long entityId)
        {
            return Activate(_finder.FindOne(Specs.Find.ById<Bargain>(entityId)));
        }
    }
}