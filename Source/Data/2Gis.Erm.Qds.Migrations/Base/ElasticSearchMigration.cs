using System;

using DoubleGis.Erm.Platform.Migration.Base;

using Nest;

namespace DoubleGis.Erm.Qds.Migrations.Base
{
    public abstract class ElasticSearchMigration : IContextedMigration<IElasticSearchMigrationContext>
    {
        public abstract void Apply(IElasticSearchMigrationContext context);

        public void Revert(IElasticSearchMigrationContext context)
        {
        }

        public static Func<CreateIndexDescriptor, CreateIndexDescriptor> GetMetadataIndexDescriptor()
        {
            return x => x
            .NumberOfShards(1)
            .NumberOfReplicas(2)
            .Settings(s => s.Add("refresh_interval", "1s"));
        }
    }
}