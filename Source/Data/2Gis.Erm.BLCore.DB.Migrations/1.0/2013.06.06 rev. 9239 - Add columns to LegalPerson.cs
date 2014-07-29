using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(9239, "Таблица Billing.LegalPersons : добавление колонки CardNumber, увеличение длинны колонки PassportNumber до 9")]
    public sealed class Migration9239 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var legalPersonTable = context.Database.GetTable(ErmTableNames.LegalPersons);

            if (legalPersonTable.Columns.Contains("CardNumber"))
            {
                return;
            }

            var passportNumberColumn = legalPersonTable.Columns["PassportNumber"];
            passportNumberColumn.DataType = DataType.NVarChar(9);
            legalPersonTable.Alter();

            var columnsToInsert = new List<InsertedColumnDefinition>
                                      {
                                          new InsertedColumnDefinition(
                                              16, x => new Column(x, "CardNumber", DataType.NVarChar(15)) { Nullable = true })
                                      };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, legalPersonTable, columnsToInsert);
        }
    }
}
