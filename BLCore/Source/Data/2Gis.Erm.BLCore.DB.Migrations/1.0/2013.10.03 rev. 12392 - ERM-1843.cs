using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12392, "ERM-1843")]
    public class Migration12392 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(@"
            DECLARE @T TABLE(Id BIGINT NOT NULL)
            INSERT INTO @T
            SELECT FileId FROM Billing.PrintFormTemplates WHERE TemplateCode IN (60, 61, 62, 63)

            DELETE FROM Billing.PrintFormTemplates
            WHERE TemplateCode IN (60, 61, 62, 63)

            DELETE FROM Shared.FileBinaries
            WHERE Id IN (SELECT Id FROM @T)

            DELETE FROM Shared.Files
            WHERE Id IN (SELECT Id FROM @T)");
        }
    }
}
