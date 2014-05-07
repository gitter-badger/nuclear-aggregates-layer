using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(18766, "Добавление mapping BusnessOperationServices поддержки выгрузки Invocies а поток floworders", "i.maslennikov")]
    public class Migration18766 : TransactedMigration
    {
        private const string CommandText = @"
DELETE FROM [Shared].[BusinessOperationServices]
WHERE Service = 14

INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], Service)
SELECT [Descriptor]
      ,[Operation]
      ,14 as Service
FROM [Shared].[BusinessOperationServices] bos 
WHERE bos.Service = 5";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(CommandText);
        }
    }
}
