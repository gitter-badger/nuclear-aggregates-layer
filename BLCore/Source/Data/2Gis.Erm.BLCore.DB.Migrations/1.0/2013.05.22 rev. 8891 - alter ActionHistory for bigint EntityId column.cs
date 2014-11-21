using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(8891, "Обновление колонки EntityId таблицы ActionsHistory")]
    public sealed class Migration8891 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var currentTimeout = context.Connection.StatementTimeout;
            context.Connection.StatementTimeout = 1200;

            const string EntityIdColumnName = "EntityId";

            var historyTable = context.Database.GetTable(ErmTableNames.ActionsHistory);
            if (historyTable != null)
            {
                if (historyTable.Indexes.Contains("IX_ActionsHistory_EntityId-EntityType"))
                {
                    context.Connection.ExecuteNonQuery(@"DROP INDEX [IX_ActionsHistory_EntityId-EntityType] ON [Shared].[ActionsHistory]");
                }

                var createdByColumn = historyTable.Columns[EntityIdColumnName];
                createdByColumn.DataType = DataType.BigInt;
                createdByColumn.Alter();

                context.Connection.ExecuteNonQuery(@"CREATE NONCLUSTERED INDEX [IX_ActionsHistory_EntityId-EntityType] ON [Shared].[ActionsHistory]
(
	[EntityId] ASC,
	[EntityType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]");

            }

            context.Connection.StatementTimeout = currentTimeout;
        }
    }
}
