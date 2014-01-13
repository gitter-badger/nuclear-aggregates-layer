using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5484, "Добавление функциональной привелегии перерасчёта лимита")]
    public class Migration5484 : TransactedMigration
    {
        const int LimitRecalculationCode = 638;

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
            serverConnection.ExecuteNonQuery(String.Format(commandTemplate, LimitRecalculationCode));
        }

        private void CreatePrivelege(ServerConnection connection)
        {
            const string commandTemplate = "INSERT INTO [Security].[Privileges]( [EntityType], [Operation]) VALUES(NULL, {0})";
            connection.ExecuteNonQuery(String.Format(commandTemplate, LimitRecalculationCode));
        }
    }
}
