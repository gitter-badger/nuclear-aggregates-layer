using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10699, "У подразделения, соответствующего УК выставляем ParentId в NULL")]
    public class Migration10699 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery("UPDATE Security.Departments SET ParentId = NULL WHERE Name = '2ГИС'");
        }
    }
}