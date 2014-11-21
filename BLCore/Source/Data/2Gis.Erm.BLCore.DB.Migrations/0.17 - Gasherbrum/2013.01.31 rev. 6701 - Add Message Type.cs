using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6701, "Добавляем сообщение для авторассыльщика")]
    public sealed class Migration6701 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var query = @"INSERT INTO [Shared].[MessageTypes](SenderSystem, ReceiverSystem, IntegrationType) VALUES ({0}, {1}, {2})";
            query = string.Format(query, 1, 6, 41); //ERM, AutoMailer, DataForAutomailer
            context.Connection.ExecuteNonQuery(query);
        }
    }
}
