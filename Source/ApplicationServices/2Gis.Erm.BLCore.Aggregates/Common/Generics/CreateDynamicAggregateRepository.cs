using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Generics
{
    public sealed class CreateDynamicAggregateRepository<TDynamicEntityInstance, TDynamicEntityPropertyInstance> :
        ICreateDynamicAggregateRepository<TDynamicEntityInstance, TDynamicEntityPropertyInstance>
        where TDynamicEntityInstance : class, IDynamicEntityInstance
        where TDynamicEntityPropertyInstance : class, INonActivityDynamicEntityPropertyInstance 
    {
        private readonly IRepository<TDynamicEntityInstance> _dynamicEntityInstanceGenericRepository;
        private readonly IRepository<TDynamicEntityPropertyInstance> _dynamicEntityPropertyInstanceGenericRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public CreateDynamicAggregateRepository(IRepository<TDynamicEntityInstance> dynamicEntityInstanceGenericRepository,
                                                IRepository<TDynamicEntityPropertyInstance> dynamicEntityPropertyInstanceGenericRepository,
                                                IIdentityProvider identityProvider,
                                                IOperationScopeFactory operationScopeFactory)
        {
            _dynamicEntityInstanceGenericRepository = dynamicEntityInstanceGenericRepository;
            _dynamicEntityPropertyInstanceGenericRepository = dynamicEntityPropertyInstanceGenericRepository;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
        }

        public long Create(TDynamicEntityInstance entityInstance, IEnumerable<TDynamicEntityPropertyInstance> propertyInstances)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, TDynamicEntityInstance>())
            {
                _identityProvider.SetFor(entityInstance);
                entityInstance.IsActive = true;

                _dynamicEntityInstanceGenericRepository.Add(entityInstance);
                operationScope.Added<TDynamicEntityInstance>(entityInstance.Id);

                _dynamicEntityInstanceGenericRepository.Save();

                foreach (var property in propertyInstances)
                {
                    _identityProvider.SetFor(property);
                    property.EntityInstanceId = entityInstance.Id;
                    _dynamicEntityPropertyInstanceGenericRepository.Add(property);
                    operationScope.Added<TDynamicEntityPropertyInstance>(property.Id);
                }

                _dynamicEntityPropertyInstanceGenericRepository.Save();

                operationScope.Complete();
                return entityInstance.Id;
            }
        }
    }
}