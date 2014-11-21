using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(7901, "Добавление функциональной привелегии каскадной смены куратора юр. лица")]
    public class Migration7901 : TransactedMigration
    {
        private const int CascadeLegalPersonAssignCode = 640;

        protected override void ApplyOverride(IMigrationContext context)
        {
            CreatePrivelege(context.Connection);
            SetupPrivelegeDepth(context.Connection);
        }

        private void SetupPrivelegeDepth(ServerConnection serverConnection)
        {
            const string commandTemplate = "declare @id int; " +
                                           "select @id = Id from [Security].[Privileges] where Operation = {0}; " +
                                           "insert into [Security].[FunctionalPrivilegeDepths](PrivilegeId, LocalResourceName, Priority, Mask) values(@id, 'FPrvDpthGranted', 1, 134)";
            serverConnection.ExecuteNonQuery(string.Format(commandTemplate, CascadeLegalPersonAssignCode));
        }

        private void CreatePrivelege(ServerConnection connection)
        {
            const string commandTemplate = "INSERT INTO [Security].[Privileges]([Id], [EntityType], [Operation]) VALUES(669, NULL, {0})"; //669 уже на продакшене. В ветке main повторяем его
            connection.ExecuteNonQuery(string.Format(commandTemplate, CascadeLegalPersonAssignCode));
        }
    }
}
