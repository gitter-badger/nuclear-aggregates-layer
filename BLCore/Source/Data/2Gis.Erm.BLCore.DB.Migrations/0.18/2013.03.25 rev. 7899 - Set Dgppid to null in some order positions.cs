using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(7899, "Убираем дубли по DgppId у позиций заказов")]
    public sealed class Migration7899 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(@"
UPDATE OP
SET DgppId = NULL
FROM Billing.OrderPositions OP 
INNER JOIN Billing.Orders O ON O.Id = OP.OrderId 
WHERE O.DgppId IS NULL AND OP.DgppId IS NOT NULL");
        }
    }
}