using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3800, "Доработка по печатным формам - обновляем таблицы безопасности")]
    public sealed class Migration3800 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"
            UPDATE [Security].[EntityTypes]
            SET Name = N'PrintFormTemplate',
	            LocalResourceName = N'EnPrintFormTemplate'
            WHERE Id = 62
            ");
        }
    }
}
