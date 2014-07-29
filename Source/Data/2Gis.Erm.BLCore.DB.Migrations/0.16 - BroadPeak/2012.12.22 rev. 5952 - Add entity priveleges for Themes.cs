using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5952, "Добавление сущностных привелегий тематики и шаблона тематики")]
    public class Migration5952 : TransactedMigration
    {
        private const string CommandTemplate = @"INSERT INTO Security.Privileges (EntityType, Operation) VALUES ({0}, 1), ({0}, 2), ({0}, 32), ({0}, 65536)";
        private const int Theme = 221;
        private const int ThemeTemplate = 222;

        protected override void ApplyOverride(IMigrationContext context)
        {
            CreatePriveleges(context.Connection, Theme);
            CreatePriveleges(context.Connection, ThemeTemplate);
        }

        private void CreatePriveleges(ServerConnection connection, int entityId)
        {
            var command = String.Format(CommandTemplate, entityId);
            connection.ExecuteNonQuery(command);
        }
    }
}
