using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    [Migration(14033, "Добавляем колонку GroupId в таблицу OrderProcessingRequestMessages", "y.baranihin")]
    public sealed class Migration14033 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string GroupIdColumnName = "GroupId";

            var table = context.Database.GetTable(ErmTableNames.OrderProcessingRequestMessages);

            if (table == null)
            {
                return;
            }

            var columnCreator = GetGuidNotNulColumnCreator(GroupIdColumnName);

            var newColumns = new[]
                {
                    new InsertedColumnDefinition(5, columnCreator)
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, newColumns);
        }

        private static Func<SqlSmoObject, Column> GetGuidNotNulColumnCreator(string columnName)
        {
            return smo => new Column(smo, columnName, DataType.UniqueIdentifier) { Nullable = false };
        }
    }
}
