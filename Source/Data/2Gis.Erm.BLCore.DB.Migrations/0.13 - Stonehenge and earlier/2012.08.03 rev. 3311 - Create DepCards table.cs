using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3311, "Добавляем таблицу DepCards")]
    public sealed class Migration3311 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddDepCardsTable(context);
            FillDepCardsTable(context);
            AddCardRelationsForeignKey(context);
            DeleteFirmContactsForeignKey(context);
            AddFirmContactsForeignKey(context);
        }

        private static void AddFirmContactsForeignKey(IMigrationContext context)
        {
            const string foreignKeyName = "FK_Contacts_DepCards";

            var table = context.Database.Tables["FirmContacts", ErmSchemas.BusinessDirectory];

            var foreignKey = table.ForeignKeys[foreignKeyName];
            if (foreignKey != null)
                return;

            // create foreign key
            foreignKey = new ForeignKey(table, foreignKeyName);
            foreignKey.Columns.Add(new ForeignKeyColumn(foreignKey, "CardId", "Code"));
            foreignKey.ReferencedTable = "DepCards";
            foreignKey.ReferencedTableSchema = ErmSchemas.Integration;
            foreignKey.Create();
        }

        private static void DeleteFirmContactsForeignKey(IMigrationContext context)
        {
            const string foreignKeyName = "FK_Contacts_CardRelations";

            var table = context.Database.Tables["FirmContacts", ErmSchemas.BusinessDirectory];

            var foreignKey = table.ForeignKeys[foreignKeyName];
            if (foreignKey == null)
                return;

            foreignKey.Drop();
        }

        private static void AddCardRelationsForeignKey(IMigrationContext context)
        {
            const string foreignKeyName = "FK_CardRelations_DepCards";

            var table = context.Database.Tables["CardRelations", ErmSchemas.Integration];

            var foreignKey = table.ForeignKeys[foreignKeyName];
            if (foreignKey != null)
                return;

            // create foreign key
            foreignKey = new ForeignKey(table, foreignKeyName);
            foreignKey.Columns.Add(new ForeignKeyColumn(foreignKey, "DepCardCode", "Code"));
            foreignKey.ReferencedTable = "DepCards";
            foreignKey.ReferencedTableSchema = ErmSchemas.Integration;
            foreignKey.Create();
        }

        private static void FillDepCardsTable(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"
            IF (EXISTS(SELECT 1 FROM Integration.DepCards))
	            RETURN
            INSERT INTO Integration.DepCards (Code)
            SELECT DepCardCode FROM Integration.CardRelations
            ");
        }

        private static void AddDepCardsTable(IMigrationContext context)
        {
            const string depCards = "DepCards";
            const string code = "Code";
            const string isHiddenOrArchived = "IsHiddenOrArchived";

            var table = context.Database.Tables[depCards, ErmSchemas.Integration];
            if (table != null)
                return;

            table = new Table(context.Database, depCards, ErmSchemas.Integration);

            var codeColumn = new Column(table, code, DataType.BigInt) {Nullable = false};
            table.Columns.Add(codeColumn);

            var isHiddenOrArchivedColumn = new Column(table, isHiddenOrArchived, DataType.Bit) { Nullable = false };
            isHiddenOrArchivedColumn.AddDefaultConstraint("DF_DepCards_IsHiddenOrArchived").Text = "0";
            table.Columns.Add(isHiddenOrArchivedColumn);

            var primaryKeyIndex = new Index(table, "PK_DepCards") {IndexKeyType = IndexKeyType.DriPrimaryKey};
            primaryKeyIndex.IndexedColumns.Add(new IndexedColumn(primaryKeyIndex, code));
            table.Indexes.Add(primaryKeyIndex);

            table.Create();
        }
    }
}
