using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(8753, "Добавление функциональной привелегии 'Администрирование номенклатуры'")]
    public class Migration8753 : TransactedMigration
    {
        private const int PrivilegeId = 670;
        private const int PositionAdministrationCode = 641;
        
        protected override void ApplyOverride(IMigrationContext context)
        {
            CreatePrivelege(context.Connection);
            SetupPrivelegeDepth(context.Connection);
        }

        private void SetupPrivelegeDepth(ServerConnection serverConnection)
        {
            const string CommandTemplate = "declare @id int; " +
                                           "select @id = Id from [Security].[Privileges] where Operation = {0}; " +
                                           "insert into [Security].[FunctionalPrivilegeDepths](PrivilegeId, LocalResourceName, Priority, Mask) values(@id, 'FPrvDpthGranted', 1, 134)";
            serverConnection.ExecuteNonQuery(string.Format(CommandTemplate, PositionAdministrationCode));
        }

        private void CreatePrivelege(ServerConnection connection)
        {
            const string CommandTemplate = "INSERT INTO [Security].[Privileges]([Id], [EntityType], [Operation]) VALUES({0}, NULL, {1})";
            connection.ExecuteNonQuery(string.Format(CommandTemplate, PrivilegeId, PositionAdministrationCode));
        }
    }
}
