using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Core;

namespace NuClear.Storage.EntityFramework
{
    public class EFDbModelConfigurationsProvider : IEFDbModelConfigurationsProvider, IEntityContainerNameResolver
    {
        private readonly IReadOnlyDictionary<Type, string> _entityTypeToContainerNameMap;
        private readonly IReadOnlyDictionary<string, IEnumerable<IEFDbModelConfiguration>> _containerNameToConfigurationsMap;
        private readonly IReadOnlyDictionary<string, IEnumerable<IEFDbModelConvention>> _containerNameToConventionsMap;

        public EFDbModelConfigurationsProvider(IReadOnlyDictionary<Type, string> entityTypeToContainerNameMap,
                                               IReadOnlyDictionary<string, IEnumerable<IEFDbModelConfiguration>> containerNameToConfigurationsMap,
                                               IReadOnlyDictionary<string, IEnumerable<IEFDbModelConvention>> containerNameToConventionsMap)
        {
            _entityTypeToContainerNameMap = entityTypeToContainerNameMap;
            _containerNameToConfigurationsMap = containerNameToConfigurationsMap;
            _containerNameToConventionsMap = containerNameToConventionsMap;
        }

        public IEnumerable<IEFDbModelConfiguration> GetConfigurations(string entityContainerName)
        {
            IEnumerable<IEFDbModelConfiguration> configurations;
            return _containerNameToConfigurationsMap.TryGetValue(entityContainerName, out configurations)
                       ? configurations
                       : Enumerable.Empty<IEFDbModelConfiguration>();
        }

        public IEnumerable<IEFDbModelConvention> GetConventions(string entityContainerName)
        {
            IEnumerable<IEFDbModelConvention> conventions;
            return _containerNameToConventionsMap.TryGetValue(entityContainerName, out conventions)
                       ? conventions
                       : Enumerable.Empty<IEFDbModelConvention>();
        }

        public string Resolve(Type entityType)
        {
            return _entityTypeToContainerNameMap[entityType];
        }
    }
}