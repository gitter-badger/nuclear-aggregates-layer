using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(8009, "Удалаяем устаревшие печатные шаблоны")]
    public sealed class Migration8009 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(
                @"UPDATE [Billing].[PrintFormTemplates] SET IsActive = 0, IsDeleted = 1, ModifiedBy = 1, ModifiedOn = GETUTCDATE() 
WHERE TemplateCode >= 50 AND TemplateCode < 58");
        }
    }
}
