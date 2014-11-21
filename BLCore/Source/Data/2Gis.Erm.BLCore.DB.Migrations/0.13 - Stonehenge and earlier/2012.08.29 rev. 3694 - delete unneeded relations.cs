using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3694, "Удаляем колонку AdvertisementId из таблицы OrderPositions")]
	public sealed class Migration3694 : TransactedMigration
	{
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropOrderPositionAdvertisementRelation(context);
            DropOrderPositionAdvrtisementIdColumn(context);
        }

        private static void DropOrderPositionAdvrtisementIdColumn(IMigrationContext context)
	    {
            var table = context.Database.Tables["OrderPositions", ErmSchemas.Billing];

            var column = table.Columns["AdvertisementId"];
            if (column == null)
                return;

            column.Drop();
	    }

	    private static void DropOrderPositionAdvertisementRelation(IMigrationContext context)
	    {
	        var table = context.Database.Tables["OrderPositions", ErmSchemas.Billing];

	        var foreignKey = table.ForeignKeys["FK_OrderPositions_Advertisement"];
            if (foreignKey == null)
                return;

            foreignKey.Drop();
	    }
	}
}