using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4167, "Добавление пользователя Integration")]
    public class Migration4167 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            object result = context.Connection.ExecuteScalar("SELECT Id FROM Security.Users WHERE Account='Integration'");
            if (result == null)
            {
                var expression = new InsertDataExpression(ErmTableNames.Users)
                    {
                        Rows = {
                                new InsertionDataDefinition()
                                    .Add("Account", "Integration")
                                    .Add("FirstName", "Integration")
                                    .Add("LastName", "")
                                    .Add("DisplayName", "Integration")
                                    .Add("DepartmentId", 1)
                                    .Add("ParentId", null)
                                    .Add("IsDeleted", 0)
                                    .Add("IsActive", 1)
                                    .Add("CreatedBy", 1)
                                    .Add("ModifiedBy", 1)
                                    .Add("CreatedOn", DateTime.Now)
                                    .Add("ModifiedOn", DateTime.Now)
                                    .Add("IsServiceUser", 1)
                            }
                    };

                context.InsertData(expression);
            }
        }
    }
}
