using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    // 2+:BL\Source\Data\2Gis.Erm.BLCore.DB.Migrations\1.2\2013.11.05 rev. 13350 - Add IsAdvertisingAgency column into clients table.cs
    [Migration(13350, "Добавляем в таблицу Clients поле IsAdvertisingAgency", "y.baranihin")]
    public class Migration13350 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string IsAdvertisingAgencyColumnName = "IsAdvertisingAgency";

            var table = context.Database.GetTable(ErmTableNames.Clients);

            if (table == null)
            {
                return;
            }

            var isAdvertisingAgencyColumnCreator = GetBitNotNulColumnCreator(IsAdvertisingAgencyColumnName);

            var newColumns = new[]
                {
                    new InsertedNotNullableColumnDefinition(13, isAdvertisingAgencyColumnCreator, IsAdvertisingAgencyColumnName, "0")
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, newColumns);
        }

        private static Func<SqlSmoObject, Column> GetBitNotNulColumnCreator(string columnName)
        {
            return smo => new Column(smo, columnName, DataType.Bit) { Nullable = false };
        }
    }
}
