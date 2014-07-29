using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(10528, "Пересчитываем поле Order.AmountWithdrawn для архивных заказов (ERM-859).")]
    public sealed class Migration10528 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(@"
UPDATE ord
SET ord.AmountWithdrawn = calculatedOrders.ADSum
FROM Billing.Orders ord
JOIN
(
SELECT o.PayableFact, o.AmountWithdrawn, s.ADSum, o.Id AS OrderId  FROM Billing.Orders o JOIN 
(SELECT OrderId, Sum(ad.Amount) as ADSum, count(*) as LocksCount FROM Billing.Locks l 
JOIN Billing.AccountDetails ad ON l.DebitAccountDetailId = ad.Id AND l.IsDeleted = 0 AND ad.IsDeleted = 0
GROUP BY OrderId) AS s ON s.OrderId = o.Id
WHERE o.WorkflowStepId = 6 AND o.OrderType = 1 AND o.AmountWithdrawn <> s.ADSum AND o.PayableFact = s.ADSum AND DgppId IS NULL AND Id <> 277493
) 
AS calculatedOrders 
ON calculatedOrders.OrderId = ord.Id");
        }
    }
}