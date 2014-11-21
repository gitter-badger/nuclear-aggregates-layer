using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._2
{
    [Migration(13031, "Добавляем в таблицу OrderProcessingRequests поля IsActive и IsDeleted")]
    public class Migration13031 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string IsActiveColumnName = "IsActive";
            const string IsDeletedColumnName = "IsDeleted";
            
            var table = context.Database.GetTable(ErmTableNames.OrderProcessingRequests);

            if (table == null)
            {
                return;
            }

            var isActiveColumnCreator = GetBitNotNulColumnCreator(IsActiveColumnName);
            var isDeletedColumnCreator = GetBitNotNulColumnCreator(IsDeletedColumnName);

            var newColumns = new[]
                {
                    new InsertedNotNullableColumnDefinition(14, isActiveColumnCreator, IsActiveColumnName, "1"), 
                    new InsertedNotNullableColumnDefinition(14, isDeletedColumnCreator, IsDeletedColumnName, "0")
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, newColumns);
        }

        private static Func<SqlSmoObject, Column> GetBitNotNulColumnCreator(string columnName)
        {
            return smo => new Column(smo, columnName, DataType.Bit) { Nullable = false };
        }
    }
}
