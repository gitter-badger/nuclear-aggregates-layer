using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201521041051, "Добавление функциональной привилегии Телефония", "a.pashkin")]
    public class Migration201521041051 : TransactedMigration
    {
        private const int TelephonyAccess = 652;
        private const string FunctionalPrivelegeGranted = "FPrvDpthGranted";

        private static readonly IDictionary<int, long> PrivelegeIds = new Dictionary<int, long>
            {
                { TelephonyAccess, 608958323585248712 },
            };

        private static readonly IDictionary<Tuple<int, string>, long> PrivelegeDepthIds = new Dictionary<Tuple<int, string>, long>
            {
                { new Tuple<int, string>(TelephonyAccess, FunctionalPrivelegeGranted), 608960603761535176 },
            };

        protected override void ApplyOverride(IMigrationContext context)
        {
            var privelegeId = PrivelegeIds[TelephonyAccess];
            var privelegeDepthId = PrivelegeDepthIds[new Tuple<int, string>(TelephonyAccess, FunctionalPrivelegeGranted)];
            CreatePrivelege(context.Connection, privelegeId, TelephonyAccess);
            SetupPrivelegeDepth(context.Connection, privelegeDepthId, privelegeId);
        }

        private void SetupPrivelegeDepth(ServerConnection serverConnection, long privelegeDepthId, long privelegeId)
        {
            const string CommandTemplate = "insert into [Security].[FunctionalPrivilegeDepths](Id, PrivilegeId, LocalResourceName, Priority, Mask) values ({0}, {1}, '{2}', 1, 134)";
            serverConnection.ExecuteNonQuery(string.Format(CommandTemplate, privelegeDepthId, privelegeId, FunctionalPrivelegeGranted));
        }

        private void CreatePrivelege(ServerConnection connection, long privelegeId, int functionalPrivelegeName)
        {
            const string CommandTemplate = "INSERT INTO [Security].[Privileges]([Id], [EntityType], [Operation]) VALUES({0}, NULL, {1})";
            connection.ExecuteNonQuery(string.Format(CommandTemplate, privelegeId, functionalPrivelegeName));
        }
    }
}
