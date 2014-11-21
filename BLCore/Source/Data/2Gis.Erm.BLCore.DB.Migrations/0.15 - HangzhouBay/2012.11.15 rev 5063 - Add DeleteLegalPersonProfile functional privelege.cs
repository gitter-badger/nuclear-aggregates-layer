using System;
using System.Text;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5063, "Добавление функциональной привелегии удаления профиля юр. лица клиента")]
    public class Migration5063 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var commandQuery = new StringBuilder();

            int deleteLegalPersonProfileFPId = 606;

            commandQuery.Append(
                "INSERT INTO [Security].[Privileges]( [EntityType], [Operation]) ")
                .Append(String.Format("VALUES(NULL, {0}) ", deleteLegalPersonProfileFPId))
                .Append("DECLARE @PrivelegeId int ")
                .Append(
                    String.Format(
                        "SET @PrivelegeId = (SELECT TOP 1 Id From [Security].[Privileges] WHERE Operation = {0}) ",
                        deleteLegalPersonProfileFPId))
                .Append("INSERT INTO [Security].[FunctionalPrivilegeDepths] ")
                .Append("( [PrivilegeId], [LocalResourceName], [Priority], [Mask]) ")
                .Append("VALUES (@PrivelegeId, 'FPrvDpthGranted', 1, 140) ");

            context.Connection.ExecuteNonQuery(commandQuery.ToString());
        }
    }
}
