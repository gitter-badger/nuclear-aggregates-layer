using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Resources;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Maintenance
{
    [Migration(25429, "Adding indexes to improve performance in activity listing.", "s.pomadin")]
    public class ListActivityMigration : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Scripts.IndexingOfReferences);
        }
    }
}