using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(15072, "Billing.ReleaseInfo.Comment and Billing.ReleaseValidationResults.Message datatype -> nvarchar(max)", "i.maslennikov")]
    public class Migration15072 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.ChangeColumnDataType(ErmTableNames.ReleaseInfos, "Comment", DataType.NVarCharMax)
                   .ChangeColumnDataType(ErmTableNames.ReleaseValidationResults, "Message", DataType.NVarCharMax);
        }
    }
}