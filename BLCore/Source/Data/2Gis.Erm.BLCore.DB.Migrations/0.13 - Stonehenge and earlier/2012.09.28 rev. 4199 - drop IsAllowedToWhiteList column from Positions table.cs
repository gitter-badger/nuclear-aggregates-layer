using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4199, "Удаление колонки IsAllowedToWhiteList из таблицы Positions")]
    public sealed class Migration4199 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropIsAllowedToWhiteListColumn(context);
            AddIsRequiredColumnToAdvertisementTemplates(context);
        }

        private static void DropIsAllowedToWhiteListColumn(IMigrationContext context)
        {
            var positionsTable = context.Database.Tables["Positions", ErmSchemas.Billing];

            var isAllowedToWhiteListColumn = positionsTable.Columns["IsAllowedToWhiteList"];
            if (isAllowedToWhiteListColumn == null)
                return;

            isAllowedToWhiteListColumn.Drop();

            var isRequiredColumn = positionsTable.Columns["IsRequired"];
            if (isRequiredColumn == null)
                return;

            isRequiredColumn.Drop();
        }

        private static void AddIsRequiredColumnToAdvertisementTemplates(IMigrationContext context)
        {
            var table = context.Database.Tables["AdvertisementTemplates", ErmSchemas.Billing];

            const string sql = "UPDATE [Billing].[AdvertisementTemplates] SET IsAdvertisementRequired = ~IsDeleted;";

            if (!table.Columns.Contains("IsAdvertisementRequired"))
            {
                var isRequiredColumn = new Column(table, "IsAdvertisementRequired", DataType.Bit) { Nullable = true };
                table.Columns.Refresh();
                table.Columns.Add(isRequiredColumn, 4);
                table.Alter();
                context.Connection.ExecuteNonQuery(sql);
                isRequiredColumn.Nullable = false;
                table.Alter();
            }
        }
    }
}
