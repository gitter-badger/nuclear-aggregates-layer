using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13639, "Добавляем функциональные привелегии для распределения горячих клиентов", "a.rechkalov")]
    public sealed class Migration13639 : TransactedMigration
    {
        private const int HotClientProcessing = 646;
        private const int HotClientTelemarketingProcessing = 647;
        private const string FunctionalPrivelegeGranted = "FPrvDpthGranted";

        private static readonly IDictionary<int, long> PrivelegeIds = new Dictionary<int, long>
            {
                { HotClientProcessing, 230714158147869960 },
                { HotClientTelemarketingProcessing, 230714261302582792 },
            };

        private static readonly IDictionary<Tuple<int, string>, long> PrivelegeDepthIds = new Dictionary<Tuple<int, string>, long>
            {
                { new Tuple<int, string>(HotClientProcessing, FunctionalPrivelegeGranted), 230722925535108616 },
                { new Tuple<int, string>(HotClientTelemarketingProcessing, FunctionalPrivelegeGranted), 230722989238198024 },
            };
        
        protected override void ApplyOverride(IMigrationContext context)
        {
            foreach (var functionalPrivelege in new[] { HotClientProcessing, HotClientTelemarketingProcessing })
            {
                var privelegeId = PrivelegeIds[functionalPrivelege];
                var privelegeDepthId = PrivelegeDepthIds[new Tuple<int, string>(functionalPrivelege, FunctionalPrivelegeGranted)];
                CreatePrivelege(context.Connection, privelegeId, functionalPrivelege);
                SetupPrivelegeDepth(context.Connection, privelegeDepthId, privelegeId);
            }
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
