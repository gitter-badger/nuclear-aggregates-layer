using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    [Migration(13789, "Отключаем проверку AdvantageousPurchasesFirmsHasBannerOrCommentValidationRule", "y.baranihin")]
    public sealed class Migration13789 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"DELETE
  FROM [Billing].[OrderValidationRuleGroupDetails] where RuleCode = 46");
        }
    }
}
