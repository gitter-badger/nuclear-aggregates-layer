using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(15115, "Billing.WithdrawalInfo.Comment datatype -> nvarchar(max)", "i.maslennikov")]
    public class Migration15115 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.ChangeColumnDataType(ErmTableNames.WithdrawalInfos, "Comment", DataType.NVarCharMax);
        }
    }
}