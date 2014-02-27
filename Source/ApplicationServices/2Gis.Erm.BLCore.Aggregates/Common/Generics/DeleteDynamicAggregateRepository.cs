using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Generics
{
    public sealed class DeleteDynamicAggregateRepository<TDynamicEntityInstance, TDynamicEntityPropertyInstance> : 
        IDeleteDynamicAggregateRepository<TDynamicEntityInstance, TDynamicEntityPropertyInstance>
        where TDynamicEntityInstance : class, IEntity, IEntityKey, IDynamicEntityInstance
        where TDynamicEntityPropertyInstance : class, IEntity, INonActivityDynamicEntityPropertyInstance
    {
        private readonly IRepository<TDynamicEntityInstance> _dynamicEntityInstanceGenericRepository;
        private readonly IRepository<TDynamicEntityPropertyInstance> _dynamicEntityPropertyInstanceGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeleteDynamicAggregateRepository(IRepository<TDynamicEntityInstance> dynamicEntityInstanceGenericRepository,
                                                IRepository<TDynamicEntityPropertyInstance> dynamicEntityPropertyInstanceGenericRepository,
                                                IOperationScopeFactory operationScopeFactory)
        {
            _dynamicEntityInstanceGenericRepository = dynamicEntityInstanceGenericRepository;
            _dynamicEntityPropertyInstanceGenericRepository = dynamicEntityPropertyInstanceGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Delete(TDynamicEntityInstance entityInstance, IEnumerable<TDynamicEntityPropertyInstance> propertyInstances)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, TDynamicEntityInstance>())
            {
                foreach (var propertyInstance in propertyInstances)
                {
                    _dynamicEntityPropertyInstanceGenericRepository.Delete(propertyInstance);
                    operationScope.Deleted<TDynamicEntityPropertyInstance>(propertyInstance.Id);
                }

                _dynamicEntityPropertyInstanceGenericRepository.Save();

                _dynamicEntityInstanceGenericRepository.Delete(entityInstance);
                operationScope.Deleted<TDynamicEntityInstance>(entityInstance.Id);

                _dynamicEntityInstanceGenericRepository.Save();

                operationScope.Complete();
            }
        }
    }
}