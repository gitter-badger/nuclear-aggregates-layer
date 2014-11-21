using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(8635, "Alter на DepartmentDelete")]
    public sealed class Migration8635 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var sp = context.Database.StoredProcedures["DepartmentDelete", ErmSchemas.Security];
            sp.TextBody = @"SET NOCOUNT ON

Declare @lft int
SELECT	@lft=LeftBorder FROM Departments WHERE ID=@i_DepartmentID and IsDeleted=0

-- Эта хранимка не меняет значения LeftBorder, RightBorder 
-- и удаляемая организация остаётся занимать прежнее место в дереве.
-- Простое смещение границ на две единицы, как это было раньше, может привести к ошибкам.
Update Departments Set IsDeleted=1, ModifiedBy=@ModifiedBy, ModifiedOn=@ModifiedOn WHERE ID=@i_DepartmentID 
";

            sp.Alter();
        }
    }
}
