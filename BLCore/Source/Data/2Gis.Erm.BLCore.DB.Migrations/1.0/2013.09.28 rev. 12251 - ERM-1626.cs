using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
   [Migration(12251, "Больше не обрабатываем операцию update для HotClientRequest")]
    public class Migration12251 : TransactedMigration
    {
       private const string DeleteStatement = @"delete from [Shared].[BusinessOperationServices] where Descriptor = 1555675434 and Operation = 31 and Service = 10";
       protected override void ApplyOverride(IMigrationContext context)
       {
           context.Database.ExecuteNonQuery(DeleteStatement);
       }
    }
}