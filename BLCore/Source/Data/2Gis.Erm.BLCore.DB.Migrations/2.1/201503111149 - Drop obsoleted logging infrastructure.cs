using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201503111149, "Удаляем все касающееся старой инфраструктуры хранения логов - таблицы и т.п.", "i.maslennikov")]
    public class Migration201503111149 : TransactedMigration, INonDefaultDatabaseMigration
    {
        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.Logging; }
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            DropStoredProcedureIfExists(context.Database, "dbo", "WriteSeanceData");
            DropStoredProcedureIfExists(context.Database, "dbo", "WriteLogMessage");

            DropViewIfExists(context.Database, "dbo", "LogView");

            DropTableIfExists(context.Database, "dbo", "LogJournal");
            DropTableIfExists(context.Database, "dbo", "UserData");
            DropTableIfExists(context.Database, "dbo", "Seances");
            DropTableIfExists(context.Database, "dbo", "Modules");
        }

        private void DropTableIfExists(Database database, string targetSchema, string targetTable)
        {
            if (database.Tables.Contains(targetTable, targetSchema))
            {
                database.Tables[targetTable, targetSchema].Drop();
            }
        }

        private void DropViewIfExists(Database database, string targetSchema, string targetView)
        {
            if (database.Views.Contains(targetView, targetSchema))
            {
                database.Views[targetView, targetSchema].Drop();
            }
        }

        private void DropStoredProcedureIfExists(Database database, string targetSchema, string targetStoredProcedure)
        {
            if (database.StoredProcedures.Contains(targetStoredProcedure, targetSchema))
            {
                database.StoredProcedures[targetStoredProcedure, targetSchema].Drop();
            }
        }
    }
}
