using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(9502, "Таблица Billing.LegalPersonsProfile : добавление сертификата о регистрации, добавление PaymentMethod в заказ")]
    public sealed class Migration9502 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var legalPersonProfileTable = context.Database.GetTable(ErmTableNames.LegalPersonProfiles);

            if (!legalPersonProfileTable.Columns.Contains("RegistrationCertificateDate"))
            {
                var columnsToInsert = new List<InsertedColumnDefinition>
                    {
                        new InsertedColumnDefinition(32, x => new Column(x, "RegistrationCertificateDate", DataType.DateTime2(2)) { Nullable = true }),
                        new InsertedColumnDefinition(32, x => new Column(x, "RegistrationCertificateNumber", DataType.NVarChar(9)) { Nullable = true })
                    };

                EntityCopyHelper.CopyAndInsertColumns(context.Database, legalPersonProfileTable, columnsToInsert);
            }

            var ordersTable = context.Database.GetTable(ErmTableNames.Orders);
            if (!ordersTable.Columns.Contains("PaymentMethod"))
            {
                var columnsToInsert = new []
                    {
                        new InsertedNotNullableColumnDefinition(47, 
                            x => new Column(x, "PaymentMethod", DataType.Int) { Nullable = false }, 
                            "PaymentMethod",
                            "0")
                    };

                EntityCopyHelper.CopyAndInsertColumns(context.Database, ordersTable, columnsToInsert);
            }
        }
    }
}
