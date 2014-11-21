using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(10107, "У некоторых заказов не выставлена платформа - исправляем")]
    public sealed class Migration10107 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(
                @"UPDATE Billing.Orders SET PlatformId = 
	CASE
		when max_platform_id!=min_platform_id then 4 
		when max_platform_id=min_platform_id then min_platform_id
	END,
	ModifiedOn = GETUTCDATE(),
	ModifiedBy = 1
  FROM [Billing].[Orders] orders
  inner join ( SELECT o.Id, min(p.PlatformId) as min_platform_id, max(p.PlatformId) as max_platform_id FROM Billing.Orders o
  inner join Billing.OrderPositions op on op.OrderId = o.Id AND op.IsActive = 1 AND op.IsDeleted = 0
  inner join Billing.PricePositions pp on op.PricePositionId = pp.Id
  inner join Billing.Positions p on pp.PositionId = p.Id
  where o.PlatformId is null and o.WorkflowStepId != 1  and o.IsActive = 1 and o.IsDeleted = 0 
  GROUP BY o.Id) as temp ON orders.Id = temp.Id");
        }
    }
}
