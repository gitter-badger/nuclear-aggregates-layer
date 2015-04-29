using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace NuClear.Storage.EntityFramework
{
    public class EFDbModelFactory : IEFDbModelFactory
    {
        private readonly object _syncRoot = new object();
        private readonly IDictionary<string, DbCompiledModel> _dbModelCache = new Dictionary<string, DbCompiledModel>();
        private readonly IEFDbModelConfigurationsProvider _efDbModelConfigurationsProvider;

        public EFDbModelFactory(IEFDbModelConfigurationsProvider efDbModelConfigurationsProvider)
        {
            _efDbModelConfigurationsProvider = efDbModelConfigurationsProvider;
        }

        public DbCompiledModel Create(string entityContainerName, DbConnection connection)
        {
            DbCompiledModel dbModel;
            lock (_syncRoot)
            {
                if (_dbModelCache.TryGetValue(entityContainerName, out dbModel))
                {
                    return dbModel;
                }

                dbModel = CreateInternal(entityContainerName, connection);
                _dbModelCache.Add(entityContainerName, dbModel);
            }

            return dbModel;
        }

        private DbCompiledModel CreateInternal(string entityContainerName, DbConnection connection)
        {
            var builder = new DbModelBuilder();

            foreach (var configuration in _efDbModelConfigurationsProvider.GetConfigurations(entityContainerName))
            {
                configuration.Apply(builder);
            }

            foreach (var convention in _efDbModelConfigurationsProvider.GetConventions(entityContainerName))
            {
                convention.Apply(builder);
            }

            return builder.Build(connection).Compile();
        }
    }
}