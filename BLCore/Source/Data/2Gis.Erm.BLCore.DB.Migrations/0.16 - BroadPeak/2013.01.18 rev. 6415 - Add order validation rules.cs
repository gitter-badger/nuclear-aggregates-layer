using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6415, "Добавляем проверки заказа")]
    public class Migration6415 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddOrderValidationRules(context);
            AddColumnToPlatforms(context);
            FillColumn(context);
        }

        private static void FillColumn(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"
            UPDATE Billing.Platforms SET IsSupportedByExport = 1 WHERE DgppId IN (0, 1, 2, 3, 4)
            ");
        }

        private static void AddColumnToPlatforms(IMigrationContext context)
        {
            var table = context.Database.Tables["Platforms", ErmSchemas.Billing];
            var column = table.Columns["IsSupportedByExport"];
            if (column != null)
            {
                return;
            }

            column = new Column(table, "IsSupportedByExport", DataType.Bit) { Nullable = false };
            var constraint = column.AddDefaultConstraint("DF_Platforms_IsSupportedByExport");
            constraint.Text = "0";
            column.Create();
        }

        private static void AddOrderValidationRules(IMigrationContext context)
        {
            var rules = new[]
            {
                new { RuleCode = 35, OrderValidationGroupId = 1 },
                new { RuleCode = 36, OrderValidationGroupId = 1 },
                new { RuleCode = 37, OrderValidationGroupId = 1 },
                new { RuleCode = 39, OrderValidationGroupId = 1 }
            };

            foreach (var rule in rules)
            {
                context.Database.ExecuteNonQuery(string.Format(@"
                IF (NOT EXISTS(SELECT * FROM Billing.OrderValidationRuleGroupDetails WHERE RuleCode = {0}))
	                INSERT INTO Billing.OrderValidationRuleGroupDetails (OrderValidationGroupId, RuleCode) VALUES ({1}, {0})
                ELSE
	                UPDATE Billing.OrderValidationRuleGroupDetails SET OrderValidationGroupId = {1} WHERE RuleCode = {0}",
                 rule.RuleCode,
                 rule.OrderValidationGroupId));
            }
        }
    }
}