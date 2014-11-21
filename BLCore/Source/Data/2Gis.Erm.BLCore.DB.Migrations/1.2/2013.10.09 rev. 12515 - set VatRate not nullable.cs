using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._2
{
    [Migration(12515, "Делаем поле VatRate обязательным для заполенения в таблице BargainTypes")]
    public sealed class Migration12515 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.BargainTypes.Name, ErmTableNames.BargainTypes.Schema];
            var column = table.Columns["VatRate"];

            if (!column.Nullable)
            {
                return;
            }

            SetValuesToNullVatRates(context);

            column.Nullable = false;
            column.Alter();
        }

        private static void SetValuesToNullVatRates(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery("Update [Billing].[BargainTypes] Set VatRate = 0 WHERE VatRate IS NULL");
        }
    }
}
