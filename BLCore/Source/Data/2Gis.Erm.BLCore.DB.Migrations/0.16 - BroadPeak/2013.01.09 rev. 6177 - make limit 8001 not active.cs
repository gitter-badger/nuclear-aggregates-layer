using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6177, "Деактивация лимита 8001")]
    public sealed class Migration6177 : TransactedMigration
    {
        private const string SqlCommand = "update Billing.Limits set IsActive = 0 where Id = 8001 and AccountId = 34758";
        protected override void ApplyOverride(IMigrationContext context)
        {
            // Наблюдается два активных лимита, привязанных к одному лицевому счету. 
            // Один из вариантов создания дублей исключён, а "лишний" лимит требуется деактивировать.
            var count = context.Connection.ExecuteNonQuery(SqlCommand);
            if(count != 1)
                throw new Exception("Не удалось деактивировать единственный лимит");
        }
    }
}
