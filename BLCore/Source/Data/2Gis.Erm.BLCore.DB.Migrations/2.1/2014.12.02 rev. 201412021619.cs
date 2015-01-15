using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201412021619, "[ERM-5372]: drop ASS related tables", "s.pomadin")]
    public class Migration201412021619 : TransactedMigration
    {
        private const string DropAssaTable = @"IF OBJECT_ID('[Billing].[AfterSaleServiceActivities]') IS NOT NULL DROP TABLE [Billing].[AfterSaleServiceActivities]";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(DropAssaTable);
        }
    }
}
