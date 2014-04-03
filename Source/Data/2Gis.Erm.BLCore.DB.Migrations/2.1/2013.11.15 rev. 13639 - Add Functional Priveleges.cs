using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(18531, "Добавляем функциональные привелегии для распределения горячих клиентов", "a.rechkalov")]
    public sealed class Migration18531 : TransactedMigration
    {
        private const int HotClientTelemarketingProcessingBranch = 649;
        private const string FunctionalPrivelegeGranted = "FPrvDpthGranted";

        private static readonly IDictionary<int, long> PrivelegeIds = new Dictionary<int, long>
            {
                { HotClientTelemarketingProcessingBranch, 324212598260111361 },
            };

        private static readonly IDictionary<Tuple<int, string>, long> PrivelegeDepthIds = new Dictionary<Tuple<int, string>, long>
            {
                { new Tuple<int, string>(HotClientTelemarketingProcessingBranch, FunctionalPrivelegeGranted), 324213019728943361 },
            };
        
        protected override void ApplyOverride(IMigrationContext context)
        {
            var privelegeId = PrivelegeIds[HotClientTelemarketingProcessingBranch];
            var privelegeDepthId = PrivelegeDepthIds[new Tuple<int, string>(HotClientTelemarketingProcessingBranch, FunctionalPrivelegeGranted)];
            CreatePrivelege(context.Connection, privelegeId, HotClientTelemarketingProcessingBranch);
            SetupPrivelegeDepth(context.Connection, privelegeDepthId, privelegeId);
        }

        private void SetupPrivelegeDepth(ServerConnection serverConnection, long privelegeDepthId, long privelegeId)
        {
            const string commandTemplate = "insert into [Security].[FunctionalPrivilegeDepths](Id, PrivilegeId, LocalResourceName, Priority, Mask) values ({0}, {1}, '{2}', 1, 134)";
            serverConnection.ExecuteNonQuery(string.Format(commandTemplate, privelegeDepthId, privelegeId, FunctionalPrivelegeGranted));
        }

        private void CreatePrivelege(ServerConnection connection, long privelegeId, int functionalPrivelegeName)
        {
            const string commandTemplate = "INSERT INTO [Security].[Privileges]([Id], [EntityType], [Operation]) VALUES({0}, NULL, {1})";
            connection.ExecuteNonQuery(string.Format(commandTemplate, privelegeId, functionalPrivelegeName));
        }
    }
}
