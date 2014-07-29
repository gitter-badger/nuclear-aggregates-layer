using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7718, "Выключаем проверку на саморекламу")]
    public sealed class Migration7718 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(@"
INSERT INTO [Billing].[OrderValidationRuleGroupDetails] ([OrderValidationGroupId], [RuleCode]) VALUES
(1, 36)
");
        }
    }
}
