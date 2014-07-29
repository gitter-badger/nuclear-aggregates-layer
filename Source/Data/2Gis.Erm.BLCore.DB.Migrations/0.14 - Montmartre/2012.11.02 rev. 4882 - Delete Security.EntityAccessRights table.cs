using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4882, "Удаляем таблицу Security.EntityAccessRights")]
    public sealed class Migration4882 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropForeignKey1(context);
            DropTable1(context);
            AddColumn1(context);
            DropColumn1(context);
        }

        private static void DropColumn1(IMigrationContext context)
        {
            var table = context.Database.Tables["Privileges", ErmSchemas.Security];

            var column = table.Columns["EntityAccessRightId"];
            if (column == null)
                return;

            column.Drop();
        }

        private static void AddColumn1(IMigrationContext context)
        {
            var table = context.Database.Tables["Privileges", ErmSchemas.Security];

            var column = table.Columns["Operation"];
            if (column != null)
                return;

            column = new Column(table, "Operation", DataType.Int) {Nullable = true};
            column.Create();

            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 1 WHERE EntityAccessRightId = 1");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 2 WHERE EntityAccessRightId = 2");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 4 WHERE EntityAccessRightId = 3");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 16 WHERE EntityAccessRightId = 4");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 32 WHERE EntityAccessRightId = 5");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 65536 WHERE EntityAccessRightId = 6");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 262144 WHERE EntityAccessRightId = 7");
            context.Database.ExecuteNonQuery(@"UPDATE Security.Privileges SET Operation = 524288 WHERE EntityAccessRightId = 8");
        }

        private static void DropForeignKey1(IMigrationContext context)
        {
            var table = context.Database.Tables["Privileges", ErmSchemas.Security];

            var key = table.ForeignKeys["FK_Privileges_AccessRights"];
            if (key != null)
            {
                key.Drop();
            }
        }

        private static void DropTable1(IMigrationContext context)
        {
            var table = context.Database.Tables["EntityAccessRights", ErmSchemas.Security];
            if (table == null)
                return;

            table.Drop();
        }
    }
}