using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(8791, "Обновление колонок CreatedBy и ModifiedBy таблиц Projects и ReleasesWithdrawalsPositions")]
    public sealed class Migration8791 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var currentTimeout = context.Connection.StatementTimeout;
            context.Connection.StatementTimeout = 1200;

            const string CreatedByColumnName = "CreatedBy";
            const string ModifiedByColumnName = "ModifiedBy";

            var projectsTable = context.Database.GetTable(ErmTableNames.Projects);
            if (projectsTable != null)
            {
                var createdByColumn = projectsTable.Columns[CreatedByColumnName];
                createdByColumn.DataType = DataType.BigInt;
                createdByColumn.Alter();

                var modifiedByColumn = projectsTable.Columns[ModifiedByColumnName];
                modifiedByColumn.DataType = DataType.BigInt;
                modifiedByColumn.Alter();
            }

            var releasesWithdrawalsPositionsTable = context.Database.GetTable(ErmTableNames.ReleasesWithdrawalsPositions);
            if (releasesWithdrawalsPositionsTable != null)
            {
                var createdByColumn = releasesWithdrawalsPositionsTable.Columns[CreatedByColumnName];
                createdByColumn.DataType = DataType.BigInt;
                createdByColumn.Alter();

                var modifiedByColumn = releasesWithdrawalsPositionsTable.Columns[ModifiedByColumnName];
                modifiedByColumn.DataType = DataType.BigInt;
                modifiedByColumn.Alter();
            }

            context.Connection.StatementTimeout = currentTimeout;
        }
    }
}
