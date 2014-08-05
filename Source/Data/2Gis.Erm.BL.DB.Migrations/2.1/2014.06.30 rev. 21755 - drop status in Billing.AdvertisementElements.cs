using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(21755, "Удаляем колонки Status и Error в [Billing].[AdvertisementElements]", "y.baranihin")]
    public class Migration21755 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(
                "INSERT INTO [Billing].[AdvertisementElementStatuses] (Id, Status, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn) " +
                "SELECT el.Id, el.Status, 1, GETUTCDATE(), 1, GETUTCDATE() FROM [Billing].[AdvertisementElements] el left join [Billing].[AdvertisementElementStatuses] s on el.Id = s.Id " +
                "WHERE s.Id IS NULL");

            var elements = context.Database.GetTable(ErmTableNames.AdvertisementElements);
            var statusColumn = elements.Columns["Status"];
            var errorColumn = elements.Columns["Error"];

            statusColumn.Drop();
            errorColumn.Drop();
        }
    }
}