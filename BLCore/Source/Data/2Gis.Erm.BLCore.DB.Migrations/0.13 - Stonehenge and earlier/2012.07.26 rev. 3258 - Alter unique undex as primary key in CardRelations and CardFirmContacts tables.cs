using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
	[Migration(3258, "Меняем unique index на primary key в таблицах CardRelations и CardFirmContacts")]
	public sealed class Migration3258: TransactedMigration
	{
		protected override void ApplyOverride(IMigrationContext context)
		{
			// CardRelations
			DropCardRelationsPrimaryKey(context);
			DropCardRelationsIdColumn(context);
			DropCardRelationsUniqueKey(context);
			AddCardRelationsPrimaryKey(context);
            AlterPosCardCodeColumnAsNullable(context);

			// normalize CardsFirmContacts
			NormalizeCardsFirmContacts(context);

            // подчищаем хвосты
            DropFirmContactsIsActiveColumn(context);
            DeleteTemporaryFirmAddress(context);
		}

        private static void DeleteTemporaryFirmAddress(IMigrationContext context)
	    {
            context.Connection.ExecuteNonQuery("DELETE FC FROM BusinessDirectory.FirmContacts FC INNER JOIN BusinessDirectory.FirmAddresses FA ON FC.FirmAddressId = FA.Id WHERE FA.Address = 'Temporary FirmAddress For Import Purposes'");
            context.Connection.ExecuteNonQuery("DELETE FROM BusinessDirectory.FirmAddresses WHERE Address = 'Temporary FirmAddress For Import Purposes'");
	    }

	    private static void DropFirmContactsIsActiveColumn(IMigrationContext context)
	    {
            const string cardRelations = "FirmContacts";
            const string isActive = "IsActive";

            var table = context.Database.Tables[cardRelations, ErmSchemas.BusinessDirectory];
            var column = table.Columns[isActive];
            if (column == null)
                return;

            column.Drop();
	    }

	    private static void NormalizeCardsFirmContacts(IMigrationContext context)
		{
			AlterFirmAddressIdColumnAsNullable(context);
			AddCardIdColumn(context);

			// то ради чего всё затевалось
			AddFirmContactsForeignKey(context);

			UpdateFirmContactsFromCardsFirmContacts(context);
		}

		private static void UpdateFirmContactsFromCardsFirmContacts(IMigrationContext context)
		{
			const string cardsFirmContacts = "CardsFirmContacts";
			var cardsFirmContactTable = context.Database.Tables[cardsFirmContacts, ErmSchemas.Integration];

			if (cardsFirmContactTable == null)
				return;

			// delete duplicates before
			context.Connection.ExecuteNonQuery("DELETE CFC FROM Integration.CardsFirmContacts CFC LEFT JOIN Integration.CardRelations CR ON CR.DepCardCode = CFC.CardCode WHERE CR.DepCardCode IS NULL");

			// insert darta from CardsFirmContacts to FirmContacts
			context.Connection.ExecuteNonQuery("UPDATE BusinessDirectory.FirmContacts SET CardId = CFC.CardCode, FirmAddressId = NULL FROM BusinessDirectory.FirmContacts FC INNER JOIN Integration.CardsFirmContacts CFC ON CFC.FirmContactId = FC.Id");

			cardsFirmContactTable.Drop();
		}

		private static void AddFirmContactsForeignKey(IMigrationContext context)
		{
			const string firmContacts = "FirmContacts";
			const string foreignKeyName = "FK_Contacts_CardRelations";

			var table = context.Database.Tables[firmContacts, ErmSchemas.BusinessDirectory];
			var foreignKey = table.ForeignKeys[foreignKeyName];
			if (foreignKey != null)
				return;

			// create foreign key
			foreignKey = new ForeignKey(table, foreignKeyName);
			foreignKey.Columns.Add(new ForeignKeyColumn(foreignKey, "CardId", "DepCardCode"));
			foreignKey.ReferencedTable = "CardRelations";
			foreignKey.ReferencedTableSchema = ErmSchemas.Integration;
			foreignKey.Create();
		}

		private static void AlterFirmAddressIdColumnAsNullable(IMigrationContext context)
		{
			const string firmContacts = "FirmContacts";
			const string firmAddressId = "FirmAddressId";

			var table = context.Database.Tables[firmContacts, ErmSchemas.BusinessDirectory];
			var column = table.Columns[firmAddressId];

			if (!column.Nullable)
				column.Nullable = true;

			column.Alter();
		}

		private static void AddCardIdColumn(IMigrationContext context)
		{
			const string firmContacts = "FirmContacts";
			const string cardId = "CardId";

			var table = context.Database.Tables[firmContacts, ErmSchemas.BusinessDirectory];
			var column = table.Columns[cardId];
			if (column != null)
				return;

			column = new Column(table, cardId, DataType.BigInt) { Nullable = true };
			column.Create();
		}

        private static void AlterPosCardCodeColumnAsNullable(IMigrationContext context)
        {
            const string cardRelations = "CardRelations";
            const string posCardCode = "PosCardCode";

            var table = context.Database.Tables[cardRelations, ErmSchemas.Integration];
            var column = table.Columns[posCardCode];

            if (!column.Nullable)
                column.Nullable = true;

            column.Alter();
        }

		private static void AddCardRelationsPrimaryKey(IMigrationContext context)
		{
			const string cardRelations = "CardRelations";
			const string primaryKeyName = "PK_CardRelations";
			const string primaryKeyColumnName = "DepCardCode";

			var table = context.Database.Tables[cardRelations, ErmSchemas.Integration];
			var index = table.Indexes[primaryKeyName];
			if (index != null)
				return;

			// create index
			index = new Index(table, primaryKeyName);
			index.IndexedColumns.Add(new IndexedColumn(index, primaryKeyColumnName));
			index.IndexKeyType = IndexKeyType.DriPrimaryKey;
			index.Create();
		}

		private static void DropCardRelationsUniqueKey(IMigrationContext context)
		{
			const string cardRelations = "CardRelations";
			const string indexName = "UQ_CardRelations_DepCardCode";

			var table = context.Database.Tables[cardRelations, ErmSchemas.Integration];
			var index = table.Indexes[indexName];
			if (index == null)
				return;

			index.Drop();
		}

		private static void DropCardRelationsIdColumn(IMigrationContext context)
		{
			const string cardRelations = "CardRelations";
			const string id = "Id";

			var table = context.Database.Tables[cardRelations, ErmSchemas.Integration];
			var column = table.Columns[id];
			if (column == null)
				return;

			column.Drop();
		}

		private static void DropCardRelationsPrimaryKey(IMigrationContext context)
		{
			const string cardRelations = "CardRelations";
			const string primaryKeyName = "PK_CardRelations";
            const string primaryKeyColumnName = "Id";

			var table = context.Database.Tables[cardRelations, ErmSchemas.Integration];
			var index = table.Indexes[primaryKeyName];
			if (index == null)
				return;

            var primaryKeyColumn = index.IndexedColumns[primaryKeyColumnName];
            if (primaryKeyColumn == null)
                return;

			index.Drop();
		}
	}
}
