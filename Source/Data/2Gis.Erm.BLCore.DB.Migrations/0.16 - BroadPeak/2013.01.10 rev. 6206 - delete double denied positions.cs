using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6206, "Удаляет повторяющиеся запрещенные позиции прайс-листа")]
    public sealed class Migration6206 : TransactedMigration
    {
        private const string SqlCommand = @"
DELETE DP1 
FROM Billing.DeniedPositions DP1, Billing.DeniedPositions DP2 
WHERE DP1.PriceId = DP2.PriceId AND 
	DP1.PositionId = DP2.PositionId AND
	DP1.PositionDeniedId = DP2.PositionDeniedId AND
	DP1.id < DP2.id
";
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(SqlCommand);
        }
    }
}
