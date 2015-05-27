using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(201504010431, "Удаление дублей правил запрещения", "y.baranihin")]
    public class Migration201504010431 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(@"Update dp set IsActive = 0, IsDeleted = 1, ModifiedBy = 1, ModifiedOn = GETUTCDATE()
                                                 from [Billing].[DeniedPositions] dp join 
                                                    (SELECT Id, Rank() over (Partition by PositionId, PriceId, PositionDeniedId, ObjectBindingType order by Id desc) as RuleRank
                                                     FROM [Billing].[DeniedPositions] where IsActive = 1 and IsDeleted = 0) as t on dp.Id = t.Id
                                                 where RuleRank > 1 ");
        }
    }
}