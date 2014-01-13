using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(10530, "Проставляем AcountDetail.OwnerCode от родительского Account.OwnerCode (фикс бага ERM-858)")]
    public sealed class Migration10530 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(@"
            UPDATE AD
            SET OwnerCode = A.OwnerCode
            FROM Billing.AccountDetails AD
            INNER JOIN Billing.Accounts A ON A.Id = AD.AccountId
            WHERE AD.OwnerCode = 0"
            );
        }
    }
}