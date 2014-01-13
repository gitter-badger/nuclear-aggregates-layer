using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11948, "Фикс ERM-1525")]
    public class Migration11948 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            // Ставим глубину "Пользователь" на операцию "Создать действие"
            context.Database.ExecuteNonQuery(@"
            UPDATE Security.RolePrivileges
            SET Mask = 1
            WHERE PrivilegeId = 78068824150115072
            ");
        }
    }
}
