using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201505070309, "Добавление ФП Смена типа заказа РА", "y.baranihin")]
    public class Migration201505070309 : TransactedMigration
    {
        private const int EditAdvertisementAgencyOrderType = 651;
        private const string FunctionalPrivelegeGranted = "FPrvDpthGranted";
        private const long PrivelegeId = 620685787322646721;
        private const long PrivelegeDepthId = 620686055011516609;

        protected override void ApplyOverride(IMigrationContext context)
        {
            CreatePrivelege(context.Connection, PrivelegeId, EditAdvertisementAgencyOrderType);
            SetupPrivelegeDepth(context.Connection, PrivelegeDepthId, PrivelegeId);
        }

        private void SetupPrivelegeDepth(ServerConnection serverConnection, long privelegeDepthId, long privelegeId)
        {
            const string CommandTemplate = "insert into [Security].[FunctionalPrivilegeDepths](Id, PrivilegeId, LocalResourceName, Priority, Mask) values ({0}, {1}, '{2}', 1, 134)";
            serverConnection.ExecuteNonQuery(string.Format(CommandTemplate, privelegeDepthId, privelegeId, FunctionalPrivelegeGranted));
        }

        private void CreatePrivelege(ServerConnection connection, long privelegeId, int functionalPrivelegeCode)
        {
            const string CommandTemplate = "INSERT INTO [Security].[Privileges]([Id], [EntityType], [Operation]) VALUES({0}, NULL, {1})";
            connection.ExecuteNonQuery(string.Format(CommandTemplate, privelegeId, functionalPrivelegeCode));
        }
    }
}