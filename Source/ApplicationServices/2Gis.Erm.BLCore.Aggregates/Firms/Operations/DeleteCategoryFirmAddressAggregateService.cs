using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class DeleteCategoryFirmAddressAggregateService : IAggregatePartRepository<Firm>, IDeleteAggregateRepository<CategoryFirmAddress>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<CategoryFirmAddress> _categoryFirmAddressRepository;
        private readonly IFirmReadModel _firmReadModel;

        public DeleteCategoryFirmAddressAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<CategoryFirmAddress> categoryFirmAddressRepository,
            IFirmReadModel firmReadModel)
        {
            _operationScopeFactory = operationScopeFactory;
            _categoryFirmAddressRepository = categoryFirmAddressRepository;
            _firmReadModel = firmReadModel;
        }

        public int Delete(CategoryFirmAddress entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, CategoryFirmAddress>())
            {
                _categoryFirmAddressRepository.Delete(entity);
                var count = _categoryFirmAddressRepository.Save();

                operationScope.Deleted<CategoryFirmAddress>(entity.Id);
                operationScope.Complete();
                return count;
            }
        }

        public int Delete(long entityId)
        {
            return Delete(_firmReadModel.GetCategoryFirmAddress(entityId));
        }
    }
}