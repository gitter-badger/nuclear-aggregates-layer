using System.Collections.Generic;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13187, "Добавлены разрешения на OrderProcessingRequest (Assign)")]
    public sealed class Migration13187 : TransactedMigration
    {
        private const string SelectTemplate = "select count(*) from [Security].[Privileges] where [EntityType] = {0} and [Operation] = {1}";
        private const string InsertTemplate = "insert into [Security].[Privileges] ([Id], [EntityType], [Operation]) values ({0}, {1}, {2})";

        private const int OrderProcessingRequest = 550;
        private const int Assign = 524288;

        static readonly IDictionary<int, long> AccessLevelIds = new Dictionary<int, long>
            {
                { Assign, 218271955676037384 },
            };

        protected override void ApplyOverride(IMigrationContext context)
        {
            foreach (var accessLevel in new[] { Assign })
            {
                AddPrivelegeIfNotExists(context.Connection, OrderProcessingRequest, accessLevel);
            }
        }

        private void AddPrivelegeIfNotExists(ServerConnection connection, int entity, int accessLevel)
        {
            var result = (int)connection.ExecuteScalar(string.Format(SelectTemplate, entity, accessLevel));
            if (result != 0)
            {
                return;
            }

            connection.ExecuteScalar(string.Format(InsertTemplate, AccessLevelIds[accessLevel], entity, accessLevel));
        }
    }
}
