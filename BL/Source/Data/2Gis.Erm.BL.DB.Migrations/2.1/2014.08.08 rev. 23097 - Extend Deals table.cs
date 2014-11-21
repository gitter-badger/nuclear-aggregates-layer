using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(23097, "расширяем таблицу [Billing].[Deals] свойствами рекламной кампании", "y.baranikhin")]
    public class Migration23097 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var dealsTable = context.Database.GetTable(ErmTableNames.Deals);
            const string BargainId = "BargainId";
            const string AdvertisingCampaignGoalText = "AdvertisingCampaignGoalText";
            const string AdvertisingCampaignGoals = "AdvertisingCampaignGoals";
            const string AdvertisingCampaignBeginDate = "AdvertisingCampaignBeginDate";
            const string AdvertisingCampaignEndDate = "AdvertisingCampaignEndDate";
            const string PaymentFormat = "PaymentFormat";
            const string AgencyFee = "AgencyFee";
            const string IdColumn = "Id";

            if (dealsTable == null)
            {
                return;
            }

            var newColumns = new[]
                {
                    new InsertedColumnDefinition(5, x => new Column(x, BargainId, DataType.BigInt) { Nullable = true }),
                    new InsertedColumnDefinition(11, x => new Column(x, AdvertisingCampaignBeginDate, DataType.DateTime2(2)) { Nullable = true }),
                    new InsertedColumnDefinition(11, x => new Column(x, AdvertisingCampaignEndDate, DataType.DateTime2(2)) { Nullable = true }),
                    new InsertedColumnDefinition(11, x => new Column(x, AdvertisingCampaignGoalText, DataType.NVarChar(512)) { Nullable = true }),
                    new InsertedColumnDefinition(11, x => new Column(x, AdvertisingCampaignGoals, DataType.Int) { Nullable = true }),
                    new InsertedColumnDefinition(11, x => new Column(x, PaymentFormat, DataType.Int) { Nullable = true }),
                    new InsertedColumnDefinition(11, x => new Column(x, AgencyFee, DataType.Decimal(4, 19)) { Nullable = true }),
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, dealsTable, newColumns);

            dealsTable = context.Database.GetTable(ErmTableNames.Deals);

            dealsTable.CreateForeignKey(BargainId, ErmTableNames.Bargains, IdColumn);
        }
    }
}