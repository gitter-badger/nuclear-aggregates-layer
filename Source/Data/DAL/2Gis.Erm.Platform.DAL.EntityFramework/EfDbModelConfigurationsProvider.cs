using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.EntityFramework;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public class EfDbModelConfigurationsProvider : IEfDbModelConfigurationsProvider, IEntityContainerNameResolver
    {
        private readonly IReadOnlyDictionary<Type, string> _entityTypeToContainerNameMap;
        private readonly IReadOnlyDictionary<string, IEnumerable<IEfDbModelConfiguration>> _containerNameToConfigurationsMap;

        public EfDbModelConfigurationsProvider(IReadOnlyDictionary<Type, string> entityTypeToContainerNameMap,
                                               IReadOnlyDictionary<string, IEnumerable<IEfDbModelConfiguration>> containerNameToConfigurationsMap)
        {
            _entityTypeToContainerNameMap = entityTypeToContainerNameMap;
            _containerNameToConfigurationsMap = containerNameToConfigurationsMap;
        }

        public IEnumerable<IEfDbModelConfiguration> GetConfigurations(string entityContainerName)
        {
            return _containerNameToConfigurationsMap[entityContainerName];
        }

        public string Resolve(Type entityType)
        {
            return _entityTypeToContainerNameMap[entityType];
        }
    }
}