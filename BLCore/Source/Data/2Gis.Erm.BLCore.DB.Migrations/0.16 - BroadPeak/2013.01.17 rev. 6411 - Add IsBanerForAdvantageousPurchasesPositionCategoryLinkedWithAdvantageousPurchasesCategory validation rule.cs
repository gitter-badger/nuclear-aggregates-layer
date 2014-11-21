﻿using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6411, "Добавление проверки наличия рубрики Выгодные покупки с 2ГИС у фирм с баннером в эту рубрику")]
    public class Migration6411 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int GroupId = 1;
            const int RuleId = 38;
            var script = string.Format(
                "INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (OrderValidationGroupId,RuleCode) VALUES ({0}, {1})", GroupId, RuleId);
            context.Connection.ExecuteNonQuery(script);
        }
    }
}
