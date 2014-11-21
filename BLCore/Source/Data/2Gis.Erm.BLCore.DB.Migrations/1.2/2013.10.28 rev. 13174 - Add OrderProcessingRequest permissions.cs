using System.Collections.Generic;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13174, "Добавлены разрешения на OrderProcessingRequest")]
    public sealed class Migration13174 : TransactedMigration
    {
        private const string SelectTemplate = "select count(*) from [Security].[Privileges] where [EntityType] = {0} and [Operation] = {1}";
        private const string InsertTemplate = "insert into [Security].[Privileges] ([Id], [EntityType], [Operation]) values ({0}, {1}, {2})";

        private const int OrderProcessingRequest = 550;
        private const int Read = 1;
        private const int Create = 32;
        private const int Update = 2;
        private const int Delete = 65536;

        static readonly IDictionary<int, long> AccessLevelIds = new Dictionary<int, long>
            {
                { Read, 217758153100493832 },
                { Create, 217758192275293448 },
                { Update, 217758229101282824 },
                { Delete, 217758268947171080 },
            };

        protected override void ApplyOverride(IMigrationContext context)
        {
            foreach (var accessLevel in new [] {Read, Create, Update, Delete})
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
