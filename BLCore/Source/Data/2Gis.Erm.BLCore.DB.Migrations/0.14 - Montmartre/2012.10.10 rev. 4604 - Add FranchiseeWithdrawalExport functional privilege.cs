using System;
using System.Text;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4604, "Добавление функциональной привелегии выгрузки списаний для франчайзи")]
    public class Migration4604 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var commandQuery = new StringBuilder();

            int FranchiseesWithdrawalExportId = 604;

            commandQuery.Append("SET IDENTITY_INSERT [Security].[Privileges]")
                .Append(
                    "ON INSERT INTO [Security].[Privileges]( [Id], [FunctionalLocalResourceName]) ")
                .Append(String.Format("VALUES( {0}, 'PrvFranchiseesWithdrawalExport') ", FranchiseesWithdrawalExportId))
                .Append("SET IDENTITY_INSERT [Security].[Privileges] OFF INSERT INTO [Security].[FunctionalPrivilegeDepths] ")
                .Append("( [PrivilegeId], [LocalResourceName], [Priority], [Mask]) ")
                .Append(String.Format("VALUES ({0}, 'FPrvDpthGranted', 1, 138) ", FranchiseesWithdrawalExportId))
                .Append("UPDATE [Security].[FunctionalPrivilegeDepths] Set Mask = 137 WHERE PrivilegeId in (SELECT Id FROM [Security].[Privileges] WHERE FunctionalLocalResourceName = 'PrvMergeLegalPersons')");

            context.Connection.ExecuteNonQuery(commandQuery.ToString());
        }
    }
}
