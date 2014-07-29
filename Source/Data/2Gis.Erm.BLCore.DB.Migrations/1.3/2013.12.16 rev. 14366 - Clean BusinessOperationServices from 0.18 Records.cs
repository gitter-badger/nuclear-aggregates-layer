using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(14366, "Очистка BusinessOperationServices от записей версии 0.18 (найдена пропущенная запись)", "a.rechkalov")]
    public class Migration114366 : TransactedMigration
    {
        private const string DeleteStatementTemplate = @"DELETE from Shared.BusinessOperationServices where Operation = {0} and Descriptor = {1} and Service = {2}";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(DeleteStatementTemplate, 14601, 653315513, 9));
        }
    }
}