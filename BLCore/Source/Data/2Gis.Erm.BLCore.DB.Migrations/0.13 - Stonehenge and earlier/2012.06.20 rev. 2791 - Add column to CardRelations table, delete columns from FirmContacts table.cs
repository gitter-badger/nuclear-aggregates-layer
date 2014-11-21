using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
	[Migration(2791, "Добавление колонок в таблицу Integration.CardRelations")]
	public class Migration2791 : TransactedMigration
	{
		protected override void ApplyOverride(IMigrationContext context)
		{
            AddColumnsToCardRelationsTable(context);
		}

        private static void AddColumnsToCardRelationsTable(IMigrationContext context)
        {
            var сardRelations = new SchemaQualifiedObjectName(ErmSchemas.Integration, "CardRelations");
            var сardRelationsTable = context.Database.GetTable(сardRelations);

            const string orderNo = "OrderNo";
            if (!сardRelationsTable.Columns.Contains(orderNo))
            {
                var column = new Column(сardRelationsTable, orderNo, DataType.Int) { Nullable = false };

                column.AddDefaultConstraint("DF_CardRelations_OrderNo").Text = "1";
                column.Create();
            }

            const string isDeleted = "IsDeleted";
            if (!сardRelationsTable.Columns.Contains(isDeleted))
            {
                var column = new Column(сardRelationsTable, isDeleted, DataType.Bit) { Nullable = false };

                column.AddDefaultConstraint("DF_CardRelations_IsDeleted").Text = "0";
                column.Create();
            }
        }
	}
}