using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class DeleteFirmContactAggregateService : IAggregatePartRepository<Firm>, IDeleteAggregateRepository<FirmContact>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<FirmContact> _firmContactRepository;
        private readonly IFirmReadModel _firmReadModel;

        public DeleteFirmContactAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<FirmContact> firmContactRepository,
            IFirmReadModel firmReadModel)
        {
            _operationScopeFactory = operationScopeFactory;
            _firmContactRepository = firmContactRepository;
            _firmReadModel = firmReadModel;
        }

        public int Delete(FirmContact entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, FirmContact>())
            {
                _firmContactRepository.Delete(entity);
                operationScope.Deleted<FirmContact>(entity.Id);

                var count = _firmContactRepository.Save();

                operationScope.Complete();

                return count;
            }
        }

        public int Delete(long entityId)
        {
            return Delete(_firmReadModel.GetFirmContact(entityId));
        }
    }
}