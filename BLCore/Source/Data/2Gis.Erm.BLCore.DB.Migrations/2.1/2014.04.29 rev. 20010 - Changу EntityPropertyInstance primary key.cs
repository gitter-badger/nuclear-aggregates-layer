using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(20010, "Меняем PrimaryKey у BusinessEntityPropertyInstances и DictionaryEntityPropertyInstances", "y.baranihin")]
    public class Migration20010 : TransactedMigration
    {
        private const string IdColumnName = "Id";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var businessEntityProperties = context.Database.GetTable(ErmTableNames.BusinessEntityPropertyInstances);
            var dictionaryEntityProperties = context.Database.GetTable(ErmTableNames.DictionaryEntityPropertyInstances);

            ChangeKey(businessEntityProperties);
            ChangeKey(dictionaryEntityProperties);
        }

        private void ChangeKey(Table table)
        {
            var primaryKey = table.Indexes.Cast<Index>().SingleOrDefault(x => x.IndexKeyType == IndexKeyType.DriPrimaryKey);
            if (primaryKey != null && primaryKey.IndexedColumns[0].Name == IdColumnName)
            {
                primaryKey.Drop();
            }

            var idColumn = table.Columns[IdColumnName];
            if (idColumn != null)
            {
                idColumn.Drop();
            }

            var index = new Index(table, string.Format("PK_{0}", table.Name))
                {
                    IndexKeyType = IndexKeyType.DriPrimaryKey
                };

            index.IndexedColumns.Add(new IndexedColumn(index, "EntityInstanceId"));
            index.IndexedColumns.Add(new IndexedColumn(index, "PropertyId"));
            table.Indexes.Add(index);

            table.Alter();
        }
    }
}
