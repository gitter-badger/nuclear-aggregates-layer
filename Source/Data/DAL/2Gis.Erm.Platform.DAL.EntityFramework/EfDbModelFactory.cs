using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public class EfDbModelFactory : IEfDbModelFactory
    {
        private readonly IEfDbModelConfigurationsProvider _efDbModelConfigurationsProvider;

        public EfDbModelFactory(IEfDbModelConfigurationsProvider efDbModelConfigurationsProvider)
        {
            _efDbModelConfigurationsProvider = efDbModelConfigurationsProvider;
        }

        public DbCompiledModel Create(string entityContainerName, DbConnection connection)
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