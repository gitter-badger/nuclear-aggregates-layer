using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(9165, "Изменение таблицы проектов")]
    public sealed class Migration9165 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddColumnsToProjectsTable(context);
            SetDefaultLangValues(context);
            SetDefaultLangNotNullable(context.Database.GetTable(ErmTableNames.Projects));
            DropDefaultLangIdColumn(context.Database.GetTable(ErmTableNames.Projects));
        }

        private static void AddColumnsToProjectsTable(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.Projects);

            if (table.Columns.Contains("DefaultLang"))
            {
                return;
            }

            var columnsToInsert = new[]
                {
                    new InsertedColumnDefinition(6, x =>
                        {
                            var c = new Column(x, "DefaultLang", DataType.NVarChar(20)) { Nullable = true };
                            return c;
                        })
                };
            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, columnsToInsert);
        }

        private static void SetDefaultLangValues(IMigrationContext context)
        {
            const string query = @"
UPDATE [Billing].[Projects] Set DefaultLang = 
CASE
WHEN DefaultLangId = 1 THEN 'ru'
WHEN DefaultLangId = 2 THEN 'en'
WHEN DefaultLangId = 3 THEN 'it'
WHEN DefaultLangId = 4 THEN 'cs'
END";
            context.Connection.ExecuteNonQuery(query);
        }

        private static void SetDefaultLangNotNullable(Table table)
        {
            var column = table.Columns["DefaultLang"];
            if (!column.Nullable)
            {
                return;
            }

            column.AddDefaultConstraint().Text = "'ru'";
            column.Nullable = false;
            column.Alter();
        }

        private static void DropDefaultLangIdColumn(Table table)
        {
            var column1 = table.Columns["DefaultLangId"];
            if (column1 != null)
            {
                column1.Drop();
            }
        }
    }
}
