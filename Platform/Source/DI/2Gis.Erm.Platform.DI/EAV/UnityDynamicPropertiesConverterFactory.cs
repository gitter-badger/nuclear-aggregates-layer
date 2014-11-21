﻿using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.EAV
{
    public sealed class UnityDynamicPropertiesConverterFactory : IDynamicPropertiesConverterFactory 
    {
        private readonly IUnityContainer _container;

        public UnityDynamicPropertiesConverterFactory(IUnityContainer container)
        {
            _container = container;
        }

        public IDynamicEntityPropertiesConverter<TEntity, TEntityInstance, TPropertyInstace> Create<TEntity, TEntityInstance, TPropertyInstace>()
            where TEntity : IEntity
            where TEntityInstance : IDynamicEntityInstance
            where TPropertyInstace : IDynamicEntityPropertyInstance
        {
            return _container.Resolve<IDynamicEntityPropertiesConverter<TEntity, TEntityInstance, TPropertyInstace>>();
        }
    }
}