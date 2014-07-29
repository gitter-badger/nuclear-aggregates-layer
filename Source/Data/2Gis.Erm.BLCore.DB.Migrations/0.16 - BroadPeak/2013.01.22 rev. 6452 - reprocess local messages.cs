using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6452, "Отправка на повторную обработку ")]
    public sealed class Migration6452 : TransactedMigration
    {
        const string SqlCommand = @"
declare @messageType int
select @messageType = Id from Shared.MessageTypes where MessageTypes.IntegrationType = 1

update Shared.LocalMessages
set Status = 2
where LocalMessages.MessageTypeId = @messageType 
	and LocalMessages.CreatedOn > '2013-01-17 17:00'
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(SqlCommand);
        }
    }
}

