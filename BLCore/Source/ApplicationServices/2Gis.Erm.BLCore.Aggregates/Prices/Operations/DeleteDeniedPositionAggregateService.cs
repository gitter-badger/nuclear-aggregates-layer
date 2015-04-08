using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class DeleteDeniedPositionAggregateService : IDeleteDeniedPositionAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<DeniedPosition> _deniedPositionRepository;

        public DeleteDeniedPositionAggregateService(IOperationScopeFactory operationScopeFactory, IRepository<DeniedPosition> deniedPositionRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _deniedPositionRepository = deniedPositionRepository;
        }

        public void Delete(DeniedPosition deniedPosition)
        {
            if (deniedPosition == null)
            {
                throw new ArgumentNullException("deniedPosition");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, DeniedPosition>())
            {
                _deniedPositionRepository.Delete(deniedPosition);
                _deniedPositionRepository.Save();

                operationScope.Deleted(deniedPosition)
                              .Complete();
            }
        }
    }
}