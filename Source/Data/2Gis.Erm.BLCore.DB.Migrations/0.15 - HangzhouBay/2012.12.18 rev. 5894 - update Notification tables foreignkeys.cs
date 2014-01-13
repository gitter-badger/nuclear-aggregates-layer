using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5894, "Исправление foreign keys на поле AddressId для таблиц NotificationEmailsTo и NotificationEmailsCc")]
    public class Migration5894 : TransactedMigration
    {
        private readonly SchemaQualifiedObjectName _notificationAddresses = new SchemaQualifiedObjectName(ErmSchemas.Shared, "NotificationAddresses");

        private readonly SchemaQualifiedObjectName _notificationEmailsTo = new SchemaQualifiedObjectName(ErmSchemas.Shared, "NotificationEmailsTo");

        private readonly SchemaQualifiedObjectName _notificationEmailsCc = new SchemaQualifiedObjectName(ErmSchemas.Shared, "NotificationEmailsCc");

        protected override void ApplyOverride(IMigrationContext context)
        {
            UpdateTableForeignKeyOnNotificationAddresses(context, _notificationEmailsTo);
            UpdateTableForeignKeyOnNotificationAddresses(context, _notificationEmailsCc);
        }

        private string GetTablePrimaryKeyColumnName(IMigrationContext context, SchemaQualifiedObjectName targetTable)
        {
            var table = context.Database.Tables[targetTable.Name, targetTable.Schema];
            var primaryKey = table.Indexes.OfType<Index>().FirstOrDefault(i => i.IndexKeyType == IndexKeyType.DriPrimaryKey);
            if (primaryKey == null || primaryKey.IndexedColumns.Count > 1)
            {
                return null;
            }

            return primaryKey.IndexedColumns[0].Name;
        }

        private void UpdateTableForeignKeyOnNotificationAddresses(IMigrationContext context, SchemaQualifiedObjectName targetTable)
        {
            var table = context.Database.Tables[targetTable.Name, targetTable.Schema];
            var targetForeignKey = table.ForeignKeys
                .OfType<ForeignKey>()
                .FirstOrDefault(key => key.ReferencedTable.Equals(targetTable.Name) && key.ReferencedTableSchema.Equals(targetTable.Schema));
            if (targetForeignKey == null || targetForeignKey.Columns.Count > 1)
            {
                return;
            }

            var keyColumnName = targetForeignKey.Columns[0].Name;

            var targetReferencedColumnName = GetTablePrimaryKeyColumnName(context, _notificationAddresses);
            if (string.IsNullOrEmpty(targetReferencedColumnName))
            {
                return;
            }

            targetForeignKey.Drop();
            table.Alter();
            table.CreateForeignKey(keyColumnName, _notificationAddresses, targetReferencedColumnName);
        }
    }
}
