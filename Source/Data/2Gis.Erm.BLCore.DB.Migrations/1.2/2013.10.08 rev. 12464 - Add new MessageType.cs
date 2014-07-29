using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12464, "Добавляем новый тип сообщения - выгрузка списаний")]
    public class Migration12464 : TransactedMigration
    {
        private const string InsertStatementTemplate = @"
IF NOT EXISTS(SELECT 1 FROM [Shared].[MessageTypes] WHERE [SenderSystem] = {1} AND [ReceiverSystem] = {2} AND [IntegrationType] = {3}) 
INSERT INTO [Shared].[MessageTypes](Id, SenderSystem, ReceiverSystem, IntegrationType)
VALUES ({0}, {1}, {2}, {3})";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 203237642798660616, 1, 4, 26));
        }
    }
}