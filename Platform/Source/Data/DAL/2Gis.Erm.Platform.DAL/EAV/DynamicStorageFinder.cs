using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.EAV;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public sealed class DynamicStorageFinder : IDynamicStorageFinder
    {
        private readonly IQuery _query;
        private readonly IDynamicPropertiesConverterFactory _converterFactory;
        private readonly IDynamicEntityMetadataProvider _dynamicEntityMetadataProvider;

        public DynamicStorageFinder(IQuery query,
                                    IDynamicPropertiesConverterFactory converterFactory,
                                    IDynamicEntityMetadataProvider dynamicEntityMetadataProvider)
        {
            _query = query;
            _converterFactory = converterFactory;
            _dynamicEntityMetadataProvider = dynamicEntityMetadataProvider;
        }

        public IEnumerable<IEntity> Find<TEntityInstance, TPropertyInstance>(SpecsBundle<TEntityInstance, TPropertyInstance> specs) 
            where TEntityInstance : class, IDynamicEntityInstance
            where TPropertyInstance : class, IDynamicEntityPropertyInstance
        {
            return _query.For(specs.FindSpec)
                         .Select(specs.SelectSpec)
                         .AsEnumerable()
                         .Select(arg => ConvertToObject(arg.EntityInstance, arg.PropertyInstances))
                         .ToArray();
        }

        private IEntity ConvertToObject<TEntityInstance, TPropertyInstance>(TEntityInstance entityInstance, ICollection<TPropertyInstance> propertyInstances)
            where TEntityInstance : class, IDynamicEntityInstance
            where TPropertyInstance : class, IDynamicEntityPropertyInstance
        {
            var typeArguments = new[]
                {
                    _dynamicEntityMetadataProvider.DetermineObjectType(propertyInstances),
                    typeof(TEntityInstance),
                    typeof(TPropertyInstance)
                };

            var type = typeof(EntityPartConverterHost<,,>).MakeGenericType(typeArguments);
            var instance = (IEntityPartConverterHost<TEntityInstance, TPropertyInstance>)Activator.CreateInstance(type, new object[] { _converterFactory });
            var obj = instance.Convert(entityInstance, propertyInstances);

            return obj;
        }
    }
}