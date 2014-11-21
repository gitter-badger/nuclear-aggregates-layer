using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5073, "Удаляем из безопасности сущность UserTerritoriesOrganizationUnits")]
    public sealed class Migration5073 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DeleteFromSecurity(context);
        }

        private static void DeleteFromSecurity(IMigrationContext context)
        {
            // EntityName.UserTerritoriesOrganizationUnits = 213
            var dataSet = context.Database.ExecuteWithResults(@"SELECT Id FROM [Security].[Privileges] WHERE EntityType = 213");
            var dataTable = dataSet.Tables[0];

            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                var row = dataTable.Rows[i];

                var privilegeId = Convert.ToInt32(row[0]);

                context.Database.ExecuteNonQuery(@"DELETE FROM [Security].[RolePrivileges] WHERE [PrivilegeId]=" + privilegeId);
            }

            context.Database.ExecuteNonQuery(@"DELETE FROM [Security].[Privileges] WHERE EntityType = 213");
        }
    }
}