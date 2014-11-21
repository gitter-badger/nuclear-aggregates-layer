using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3514, "Добавление колонок \"Дата перехода на ERM\" и \"Дата перехода на IR\" в таблицу Billing.OrganizationUnits. " +
                            "Удаление колонки \"Переведен на CRM\"")]
    public class Migration3514 : TransactedMigration
    {
        const string ErmLaunchDate = "ErmLaunchDate";
        const string InfoRussiaLaunchDate = "InfoRussiaLaunchDate";
        const string IsMovedToCrm = "IsMovedToCrm";

        #region SQL Statements

        private const string SetErmLaunchDateToOrganizationUnitsMovedToCrm = @"
UPDATE OU 
SET OU.ErmLaunchDate = '{0}'
FROM [Billing].[OrganizationUnits] AS OU
WHERE IsMovedToCrm = 1";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
		{
            var table = context.Database.GetTable(ErmTableNames.OrganizationUnits);

            table = AddErmLaunchDateAndInfoRussiaLaunchDateColumns(context.Database, table);
            if (table.Columns.Contains(ErmLaunchDate) && table.Columns.Contains(InfoRussiaLaunchDate) && table.Columns.Contains(IsMovedToCrm))
            {
                FillErmLaunchDateColumn(context.Connection, table);
                DropIsMovedToCrmColumn(table);
            }
		}

        private static Table AddErmLaunchDateAndInfoRussiaLaunchDateColumns(Database database, Table table)
        {
            if (!table.Columns.Contains(ErmLaunchDate) && !table.Columns.Contains(InfoRussiaLaunchDate))
            {
                var ermLaunchDateColumn = new InsertedColumnDefinition(
                    10, x => new Column(x, ErmLaunchDate, DataType.DateTime2(2)) {Nullable = true});
                var infoRussiaLaunchDateColumn = new InsertedColumnDefinition(
                    11, x => new Column(x, InfoRussiaLaunchDate, DataType.DateTime2(2)) {Nullable = true});
                return EntityCopyHelper.CopyAndInsertColumns(database, table,
                                                      new[] {ermLaunchDateColumn, infoRussiaLaunchDateColumn});
            }
            return table;
        }

        private static void FillErmLaunchDateColumn(ServerConnection connection, Table table)
        {
            const string defaultLaunchDate = "2011-12-01";
            var sql = string.Format(SetErmLaunchDateToOrganizationUnitsMovedToCrm, defaultLaunchDate);
            connection.ExecuteNonQuery(sql);
        }

        private static void DropIsMovedToCrmColumn(Table table)
        {
            table.Columns[IsMovedToCrm].Drop();
        }
	}
}