using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12479, "Создать vwSecurityAccelerator")]
    public class Migration12479 : TransactedMigration
    {
        private const string InsertStatements = @"
CREATE view [Security].[vwSecurityAccelerator]
WITH SCHEMABINDING
AS
SELECT Users.Id as UserId,
       Departments.Id as DepartmentId,
	   Departments.LeftBorder as DepartmentLeftBorder,
	   Departments.RightBorder as DepartmentRightBorder
FROM Security.Users
	inner join Security.Departments on Departments.Id = Users.DepartmentId

GO

CREATE UNIQUE CLUSTERED INDEX PK_SecurityAcceleration ON [Security].[vwSecurityAccelerator] (UserId)
GO
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
