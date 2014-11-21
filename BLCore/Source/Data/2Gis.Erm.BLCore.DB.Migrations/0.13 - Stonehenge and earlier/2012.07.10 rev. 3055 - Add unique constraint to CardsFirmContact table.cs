using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3055, "Удаляем данные дубликаты и добавляем constraint уникальности в таблицу CardsFirmContacts")]
    public class Migration3055 : TransactedMigration
    {
        private static readonly SchemaQualifiedObjectName TableName = new SchemaQualifiedObjectName(ErmSchemas.Integration , "CardsFirmContacts");
        private const string ColumnName = "FirmContactId";

        protected override void ApplyOverride(IMigrationContext context)
        {
            DeleteDuplicates(context);
            CreateUniqueIndex(context);
        }

        private static void DeleteDuplicates(IMigrationContext context)
        {
            var sql = string.Format(CultureInfo.InvariantCulture, "DELETE FROM {0}.{1} WHERE {2} IN (SELECT {2} FROM {0}.{1} GROUP BY {2} HAVING COUNT(*) > 1)", TableName.Schema, TableName.Name, ColumnName);
            context.Connection.ExecuteNonQuery(sql);
        }

        private static void CreateUniqueIndex(IMigrationContext context)
        {
            var indexName = "UQ_" + TableName.Name + "_" + ColumnName;

            var table = context.Database.Tables[TableName.Name, TableName.Schema];

            // do nothing if index already exists
            if (table.Indexes.Cast<Index>().Any(x => x.Name == indexName))
                return;

            // create index
            var index = new Index(table, indexName);
            index.IndexedColumns.Add(new IndexedColumn(index, ColumnName));
            index.IndexKeyType = IndexKeyType.DriUniqueKey;
            index.Create();
        }
    }
}