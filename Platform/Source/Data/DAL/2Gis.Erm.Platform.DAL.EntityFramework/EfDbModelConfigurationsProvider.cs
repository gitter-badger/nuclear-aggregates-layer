using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.EntityFramework;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public class EfDbModelConfigurationsProvider : IEfDbModelConfigurationsProvider, IEntityContainerNameResolver
    {
        private readonly IReadOnlyDictionary<Type, string> _entityTypeToContainerNameMap;
        private readonly IReadOnlyDictionary<string, IEnumerable<IEfDbModelConfiguration>> _containerNameToConfigurationsMap;
        private readonly IReadOnlyDictionary<string, IEnumerable<IEfDbModelConvention>> _containerNameToConventionsMap;

        public EfDbModelConfigurationsProvider(IReadOnlyDictionary<Type, string> entityTypeToContainerNameMap,
                                               IReadOnlyDictionary<string, IEnumerable<IEfDbModelConfiguration>> containerNameToConfigurationsMap,
                                               IReadOnlyDictionary<string, IEnumerable<IEfDbModelConvention>> containerNameToConventionsMap)
        {
            _entityTypeToContainerNameMap = entityTypeToContainerNameMap;
            _containerNameToConfigurationsMap = containerNameToConfigurationsMap;
            _containerNameToConventionsMap = containerNameToConventionsMap;
        }

        public IEnumerable<IEfDbModelConfiguration> GetConfigurations(string entityContainerName)
        {
            IEnumerable<IEfDbModelConfiguration> configurations;
            return _containerNameToConfigurationsMap.TryGetValue(entityContainerName, out configurations)
                       ? configurations
                       : Enumerable.Empty<IEfDbModelConfiguration>();
        }

        public IEnumerable<IEfDbModelConvention> GetConventions(string entityContainerName)
        {
            IEnumerable<IEfDbModelConvention> conventions;
            return _containerNameToConventionsMap.TryGetValue(entityContainerName, out conventions)
                       ? conventions
                       : Enumerable.Empty<IEfDbModelConvention>();
        }

        public string Resolve(Type entityType)
        {
            return _entityTypeToContainerNameMap[entityType];
        }
    }
}