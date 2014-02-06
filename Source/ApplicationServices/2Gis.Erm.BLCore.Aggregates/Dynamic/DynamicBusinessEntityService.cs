using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Dynamic
{
    public class DynamicBusinessEntityService<TDynamicEntityInstance, TDynamicPropertyEntityInstance> : IDynamicBusinessEntityService<TDynamicEntityInstance, TDynamicPropertyEntityInstance>
        where TDynamicEntityInstance : class, IEntity, IEntityKey, IDynamicEntityInstance
        where TDynamicPropertyEntityInstance : class, IEntity, INonActivityDynamicEntityPropertyInstance
    {
        private readonly IRepository<TDynamicEntityInstance> _dynamicEntityInstanceGenericRepository;
        private readonly IRepository<TDynamicPropertyEntityInstance> _dynamicEntityPropertyInstanceGenericRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DynamicBusinessEntityService(
            IRepository<TDynamicEntityInstance> dynamicEntityInstanceGenericRepository,
            IRepository<TDynamicPropertyEntityInstance> dynamicEntityPropertyInstanceGenericRepository,
            IIdentityProvider identityProvider,
            IOperationScopeFactory operationScopeFactory)
        {
            _dynamicEntityInstanceGenericRepository = dynamicEntityInstanceGenericRepository;
            _dynamicEntityPropertyInstanceGenericRepository = dynamicEntityPropertyInstanceGenericRepository;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Create(TDynamicEntityInstance entity)
        {
            throw new System.NotImplementedException();
        }

        public int Update(TDynamicEntityInstance entity)
        {
            throw new System.NotImplementedException();
        }

        public int Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public long Create(TDynamicEntityInstance entityInstance, IEnumerable<TDynamicPropertyEntityInstance> propertyInstances)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, TDynamicEntityInstance>())
            {
                _identityProvider.SetFor(entityInstance);
                _dynamicEntityInstanceGenericRepository.Add(entityInstance);
                operationScope.Added<TDynamicEntityInstance>(entityInstance.Id);

                _dynamicEntityInstanceGenericRepository.Save();

                foreach (var property in propertyInstances)
                {
                    _identityProvider.SetFor(property);
                    property.EntityInstanceId = entityInstance.Id;
                    _dynamicEntityPropertyInstanceGenericRepository.Add(property);
                    operationScope.Added<TDynamicPropertyEntityInstance>(property.Id);
                }

                _dynamicEntityPropertyInstanceGenericRepository.Save();

                operationScope.Complete();
                return entityInstance.Id;
            }
        }

        public void Update(TDynamicEntityInstance entityInstance, IEnumerable<TDynamicPropertyEntityInstance> propertyInstances)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, TDynamicEntityInstance>())
            {
                _dynamicEntityInstanceGenericRepository.Update(entityInstance);
                operationScope.Updated<TDynamicEntityInstance>(entityInstance.Id);

                _dynamicEntityInstanceGenericRepository.Save();

                foreach (var property in propertyInstances)
                {
                    if (property.IsNew())
                    {
                        _identityProvider.SetFor(property);
                        property.EntityInstanceId = entityInstance.Id;
                        _dynamicEntityPropertyInstanceGenericRepository.Add(property);
                        operationScope.Added<TDynamicPropertyEntityInstance>(property.Id);
                    }
                    else
                    {
                        _dynamicEntityPropertyInstanceGenericRepository.Update(property);
                        operationScope.Updated<TDynamicPropertyEntityInstance>(property.Id);
                    }
                }

                _dynamicEntityPropertyInstanceGenericRepository.Save();

                operationScope.Complete();
            }
        }
    }
}