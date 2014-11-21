using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5154, "Удаление колонок из таблиц, ранее используемых для интегации с Экспортом")]
    public sealed class Migration5154 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AlterReleaseInfoTable(context.Database);
            AlterOrganizationUnitTable(context.Database);
        }

        private static void AlterReleaseInfoTable(Database database)
        {
            var table = database.Tables[ErmTableNames.ReleaseInfos.Name, ErmTableNames.ReleaseInfos.Schema];
            var hashColumn = table.Columns["Hash"];
            if (hashColumn != null)
            {
                hashColumn.Drop();
            }
            
        }

        private static void AlterOrganizationUnitTable(Database database)
        {
            var table = database.Tables[ErmTableNames.OrganizationUnits.Name, ErmTableNames.OrganizationUnits.Schema];
            var exportFtpFolderNameColumn = table.Columns["ExportFtpFolderName"];
            if (exportFtpFolderNameColumn != null)
            {
                exportFtpFolderNameColumn.Drop();
            }
        }
    }
}