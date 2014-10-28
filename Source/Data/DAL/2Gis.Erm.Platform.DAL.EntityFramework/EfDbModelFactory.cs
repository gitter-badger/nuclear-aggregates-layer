using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

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
                configuration.ApplyConfiguration(builder);
            }

            return builder.Build(connection).Compile();
        }
    }
}