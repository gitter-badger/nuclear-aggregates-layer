using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5869, "Отправка на повторную обработку ")]
    public sealed class Migration5869 : TransactedMigration
    {
        const string SqlCommand = @"
declare @messageType int
select @messageType = Id from Shared.MessageTypes where MessageTypes.IntegrationType = 1

update Shared.LocalMessages
set Status = 2
where LocalMessages.MessageTypeId = @messageType 
	and LocalMessages.CreatedOn > '2012-12-17 15:37'
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(SqlCommand);
        }
    }
}
