using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6449, "Добавление табличного типа для передачи идентификаторов")]
    public class Migration6449 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string Query = @"IF TYPE_ID(N'[Shared].[Int32IdsTableType]') IS NULL 
                                    CREATE TYPE [Shared].[Int32IdsTableType] AS TABLE([Id] [int] NOT NULL)";
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}
