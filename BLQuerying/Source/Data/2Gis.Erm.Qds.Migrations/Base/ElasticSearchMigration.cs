using DoubleGis.Erm.Platform.Migration.Base;

namespace DoubleGis.Erm.Qds.Migrations.Base
{
    public abstract class ElasticSearchMigration : IContextedMigration<IElasticSearchMigrationContext>
    {
        public abstract void Apply(IElasticSearchMigrationContext context);

        public void Revert(IElasticSearchMigrationContext context)
        {
        }
    }
}