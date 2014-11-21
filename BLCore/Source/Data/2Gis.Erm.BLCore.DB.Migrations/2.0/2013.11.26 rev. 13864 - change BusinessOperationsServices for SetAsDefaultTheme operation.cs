using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13864, "Актуализация BusnessOperationServices для операции SetAsDefaultTheme")]
    public class Migration13864 : TransactedMigration
    {
        private const string CommandText = @"
-- Актуализация операции SetAsDefaultTheme - была ошибочно объявлена как entityspecific, теперь стала noncoupled
DELETE FROM [Shared].[BusinessOperationServices] WHERE [Descriptor] = -1586871658 and [Operation] = 29 and [Service] = 7
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (0, 29, 7)
go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(CommandText);
        }
    }
}
