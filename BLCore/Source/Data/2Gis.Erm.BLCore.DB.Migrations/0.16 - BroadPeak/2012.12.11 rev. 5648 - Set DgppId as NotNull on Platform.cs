using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5648, "Делаем DgppId как not null в таблице Platforms")]
    public sealed class Migration5648 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            MakePlatformDgppIdNotNull(context);
        }

        private static void MakePlatformDgppIdNotNull(IMigrationContext context)
        {
            const string indexName = "UQ_Platforms_DgppId";

            var table = context.Database.Tables["Platforms", ErmSchemas.Billing];
            var column = table.Columns["DgppId"];

            if (!column.Nullable)
            {
                return;
            }

            DeleteNullPlatforms(context);

            column.Nullable = false;
            column.Alter();

            AddUniqueIndex(table, indexName);
        }

        private static void DeleteNullPlatforms(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery("DELETE FROM Billing.Platforms WHERE DgppId IS NULL");
        }

        private static void AddUniqueIndex(TableViewTableTypeBase table, string indexName)
        {
            var index = table.Indexes[indexName];
            if (index != null)
            {
                return;
            }

            index = new Index(table, indexName) { IndexKeyType = IndexKeyType.DriUniqueKey };
            index.IndexedColumns.Add(new IndexedColumn(index, "DgppId"));

            index.Create();
        }
    }
}
