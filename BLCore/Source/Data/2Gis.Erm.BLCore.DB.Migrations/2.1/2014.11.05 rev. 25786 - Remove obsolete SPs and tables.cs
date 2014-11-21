using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(25786, "Удаление не используемых хранимых процедур, таблиц и т.п.", "i.maslennikov")]
    public class Migration25786 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropCreateExportSessionSP(context);
            DropBusinessOperationsTable(context);
        }

        private void DropCreateExportSessionSP(IMigrationContext context)
        {
            var targetStoredProcedure = context.Database.StoredProcedures[ErmStoredProcedures.CreateExportSession.Name, ErmStoredProcedures.CreateExportSession.Schema];
            if (targetStoredProcedure == null)
            {
                return;
            }

            targetStoredProcedure.Drop();
        }

        private void DropBusinessOperationsTable(IMigrationContext context)
        {
            var targetTable = context.Database.Tables[ErmTableNames.BusinessOperations.Name, ErmTableNames.BusinessOperations.Schema];
            if (targetTable == null)
            {
                return;
            }

            targetTable.Drop();
        }
    }
}