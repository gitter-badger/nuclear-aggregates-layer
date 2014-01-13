using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(8897, "Добавление колонок в таблицы LegalPersons и LegalPersonProfiles")]
    public sealed class Migration8897 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var legalPersonTable = context.Database.GetTable(ErmTableNames.LegalPersons);

            if (legalPersonTable.Columns.Contains("VAT"))
            {
                return;
            }

            var columnsToInsert = new List<InsertedColumnDefinition>
                                      {
                                          new InsertedColumnDefinition(
                                              10, x => new Column(x, "VAT", DataType.NVarChar(11)) { Nullable = true })
                                      };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, legalPersonTable, columnsToInsert);

            var profilesTable = context.Database.GetTable(ErmTableNames.LegalPersonProfiles);

            if (profilesTable.Columns.Contains("PaymentMethod"))
            {
                return;
            }

            var columnsToInsertToProfiles = new List<InsertedColumnDefinition>
                                      {
                                          new InsertedColumnDefinition(
                                              10, x => new Column(x, "PaymentMethod", DataType.Int) { Nullable = true }),
                                              new InsertedColumnDefinition(
                                              10, x => new Column(x, "AccountNumber", DataType.NVarChar(16)) { Nullable = true }),
                                              new InsertedColumnDefinition(
                                              10, x => new Column(x, "IBAN", DataType.NVarChar(28)) { Nullable = true }),
                                              new InsertedColumnDefinition(
                                              10, x => new Column(x, "SWIFT", DataType.NVarChar(11)) { Nullable = true }),
                                              new InsertedColumnDefinition(
                                              10, x => new Column(x, "BankName", DataType.NVarChar(100)) { Nullable = true }),
                                              new InsertedColumnDefinition(
                                              10, x => new Column(x, "AdditionalPaymentElements", DataType.NVarChar(512)) { Nullable = true }),
                                      };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, profilesTable, columnsToInsertToProfiles);
        }
    }
}
