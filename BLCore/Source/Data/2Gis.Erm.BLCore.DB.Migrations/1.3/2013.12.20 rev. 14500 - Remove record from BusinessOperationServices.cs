using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    [Migration(14500, "Удаляем обработку операции CopyOrderIdentity из BusinessOperationServices ", "y.baranihin")]
    public class Migration14500 : TransactedMigration
    {
        private const string DeleteStatementTemplate =
            @"DELETE from Shared.BusinessOperationServices where Operation = {0} and Descriptor = {1} and Service = {2}";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(DeleteStatementTemplate, 15113, 0, 5));
        }
    }
}
