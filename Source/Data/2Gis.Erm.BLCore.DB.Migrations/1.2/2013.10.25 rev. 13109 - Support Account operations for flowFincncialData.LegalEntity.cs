using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13109, "Выгрузка юрлиц при изменении лицевых счетов")]
    public class Migration13109 : TransactedMigration
    {
        private const string InsertStatementTemplate = @"
IF NOT EXISTS(SELECT 1 FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2})
INSERT INTO [Shared].[BusinessOperationServices](Descriptor, Operation, Service)
VALUES ({0}, {1}, {2})
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, -655314502, 30, 3));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, -655314502, 31, 3));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, -655314502, 9, 3));
        }
    }
}