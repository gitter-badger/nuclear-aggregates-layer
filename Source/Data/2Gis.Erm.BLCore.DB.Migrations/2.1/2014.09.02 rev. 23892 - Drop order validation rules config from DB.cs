using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(23892, "Удаления конфигурации правил проверок заказов из БД", "i.maslennikov")]
    public sealed class Migration23892 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropTables(context, ErmTableNames.OrderValidationRuleGroupDetails, ErmTableNames.OrderValidationRuleGroups);
        }

        private void DropTables(IMigrationContext context, params SchemaQualifiedObjectName[] tables)
        {
            var targetTables = 
                tables.Select(t => context.Database.Tables[t.Name, t.Schema])
                      .Where(t => t != null)
                      .ToArray();

            foreach (var table in targetTables)
            {
                table.Drop();
            }

            context.Database.Alter();
        }
    }
}
