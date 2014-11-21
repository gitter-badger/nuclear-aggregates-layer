using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5256, "Избавляемся от нулевых значений в колонке LastQualifyTime в таблице Clients.")]
    public sealed class Migration5256 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var clientsTable = context.Database.GetTable(ErmTableNames.Clients);

            var column = clientsTable.Columns["LastQualifyTime"];

            if (column.Nullable)
            {
                context.Connection.ExecuteNonQuery(
                    string.Format("UPDATE {0} SET LastQualifyTime = CreatedOn WHERE LastQualifyTime IS NULL", ErmTableNames.Clients));
                column.Nullable = false;
                column.Alter();
            }
        }
    }
}
