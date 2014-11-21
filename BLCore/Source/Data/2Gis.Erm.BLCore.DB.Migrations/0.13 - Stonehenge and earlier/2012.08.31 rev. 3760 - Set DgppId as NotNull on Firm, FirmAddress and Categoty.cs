using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3760, "делаем DgppId not null в таблицах Firm, FirmAddress и Category")]
    public sealed class Migration3760 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            MakeFirmDgppIdNotNull(context);
            MakeFirmAddressDgppIdNotNull(context);
            MakeCategoriesDgppIdNotNull(context);
        }

        private static void MakeCategoriesDgppIdNotNull(IMigrationContext context)
        {
            const string indexName = "UQ_Categories_DgppId";

            var table = context.Database.Tables["Categories", ErmSchemas.BusinessDirectory];
            var column = table.Columns["DgppId"];

            if (!column.Nullable)
                return;

            DropUniqueIndex(table, indexName);

            column.Nullable = false;
            column.Alter();

            AddUniqueIndex(table, indexName);
        }

        private static void MakeFirmAddressDgppIdNotNull(IMigrationContext context)
        {
            const string indexName = "UQ_FirmAddresses_DgppId";

            var table = context.Database.Tables["FirmAddresses", ErmSchemas.BusinessDirectory];
            var column = table.Columns["DgppId"];

            if (!column.Nullable)
                return;

            DropForeignKey(context);
            DropUniqueIndex(table, indexName);

            column.Nullable = false;
            column.Alter();

            AddUniqueIndex(table, indexName);
            AddForeignKey(context);
        }

        private static void AddForeignKey(IMigrationContext context)
        {
            const string foreignKeyName = "FK_CardRelations_FirmAddresses";
            var table = context.Database.Tables["CardRelations", ErmSchemas.Integration];

            var foreignKey = table.ForeignKeys[foreignKeyName];
            if (foreignKey != null)
                return;

            // create foreign key
            foreignKey = new ForeignKey(table, foreignKeyName);
            foreignKey.Columns.Add(new ForeignKeyColumn(foreignKey, "PosCardCode", "DgppId"));
            foreignKey.ReferencedTable = "FirmAddresses";
            foreignKey.ReferencedTableSchema = ErmSchemas.BusinessDirectory;
            foreignKey.Create();
        }

        private static void DropForeignKey(IMigrationContext context)
        {
            const string foreignKeyName = "FK_CardRelations_FirmAddresses";
            var table = context.Database.Tables["CardRelations", ErmSchemas.Integration];

            var foreignKey = table.ForeignKeys[foreignKeyName];
            if (foreignKey == null)
                return;

            foreignKey.Drop();
        }

        private static void MakeFirmDgppIdNotNull(IMigrationContext context)
        {
            const string indexName = "UQ_Firms_DgppId";

            var table = context.Database.Tables["Firms", ErmSchemas.BusinessDirectory];
            var column = table.Columns["DgppId"];

            if(!column.Nullable)
                return;

            DropUniqueIndex(table, indexName);

            column.Nullable = false;
            column.Alter();

            AddUniqueIndex(table, indexName);
        }

        private static void DropUniqueIndex(TableViewTableTypeBase table, string indexName)
        {
            var index = table.Indexes[indexName];
            if (index == null)
                return;

            index.Drop();
        }

        private static void AddUniqueIndex(TableViewTableTypeBase table, string indexName)
        {
            var index = table.Indexes[indexName];
            if (index != null)
                return;

            index = new Index(table, indexName) { IndexKeyType = IndexKeyType.DriUniqueKey };
            index.IndexedColumns.Add(new IndexedColumn(index, "DgppId"));

            index.Create();
        }
    }
}
