using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Generics
{
    public sealed class UpdateDynamicAggregateRepository<TDynamicEntityInstance, TDynamicEntityPropertyInstance> :
        IUpdateDynamicAggregateRepository<TDynamicEntityInstance, TDynamicEntityPropertyInstance>
        where TDynamicEntityInstance : class, IEntity, IEntityKey, IDynamicEntityInstance
        where TDynamicEntityPropertyInstance : class, IEntity, INonActivityDynamicEntityPropertyInstance
    {
        private readonly IRepository<TDynamicEntityInstance> _dynamicEntityInstanceGenericRepository;
        private readonly IRepository<TDynamicEntityPropertyInstance> _dynamicEntityPropertyInstanceGenericRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public UpdateDynamicAggregateRepository(IRepository<TDynamicEntityInstance> dynamicEntityInstanceGenericRepository,
                                                IRepository<TDynamicEntityPropertyInstance> dynamicEntityPropertyInstanceGenericRepository,
                                                IIdentityProvider identityProvider,
                                                IOperationScopeFactory operationScopeFactory)
        {
            _dynamicEntityInstanceGenericRepository = dynamicEntityInstanceGenericRepository;
            _dynamicEntityPropertyInstanceGenericRepository = dynamicEntityPropertyInstanceGenericRepository;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Update(TDynamicEntityInstance entityInstance, IEnumerable<TDynamicEntityPropertyInstance> propertyInstances)
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
                        operationScope.Added<TDynamicEntityPropertyInstance>(property.Id);
                    }
                    else
                    {
                        _dynamicEntityPropertyInstanceGenericRepository.Update(property);
                        operationScope.Updated<TDynamicEntityPropertyInstance>(property.Id);
                    }
                }

                _dynamicEntityPropertyInstanceGenericRepository.Save();

                operationScope.Complete();
            }
        }
    }
}