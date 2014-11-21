using System;
using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10738, "Отключаем автоинкремент в таблицах RegionalAdvertisingSharings, OrdersRegionalAdvertisingSharings, " +
                      "Notes, NotificationEmails, NotificationAddresses, NotificationEmailsTo, NotificationProcessings")]
    public class Migration10738 : TransactedMigration
    {
        private static readonly SchemaQualifiedObjectName[] Tables =
        {
            ErmTableNames.RegionalAdvertisingSharings,
            ErmTableNames.OrdersRegionalAdvertisingSharings,
            ErmTableNames.Notes,
            ErmTableNames.NotificationEmails,
            ErmTableNames.NotificationAddresses,
            ErmTableNames.NotificationEmailsTo,
            ErmTableNames.NotificationProcessings
        };

        protected override void ApplyOverride(IMigrationContext context)
        {
            var tables = Tables.Select(name => context.Database.GetTable(name))
                               .Where(table => table.Columns["Id"].Identity)
                               .ToArray();

            foreach (var table in tables)
            {
                try
                {
                    EntityCopyHelper.RemoveIdentity(context.Database, table);
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("While processing {0}", table.Name), e);
                }
            }
        }
    }
}