using System;
using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
	[Migration(3249, "Добавление constraints к таблице CardRelations")]
	public sealed class Migration3249: TransactedMigration
	{
		protected override void ApplyOverride(IMigrationContext context)
		{
			CreateUniqueIndexOnDepCards(context);
			CreateForeignKeyOnPosCards(context);
		}

		private static void CreateForeignKeyOnPosCards(IMigrationContext context)
		{
			// delete duplicates
			context.Connection.ExecuteNonQuery("DELETE CR FROM Integration.CardRelations CR LEFT JOIN BusinessDirectory.FirmAddresses FA ON CR.PosCardCode = FA.DgppId WHERE FA.DgppId IS NULL");

			const string foreignKeyName = "FK_CardRelations_FirmAddresses";
			var table = context.Database.Tables["CardRelations", ErmSchemas.Integration];

			if (table.ForeignKeys.Cast<Index>().Any(x => string.Equals(x.Name, foreignKeyName, StringComparison.OrdinalIgnoreCase)))
				return;

			// create foreign key
			var foreignKey = new ForeignKey(table, foreignKeyName);
			foreignKey.Columns.Add(new ForeignKeyColumn(foreignKey, "PosCardCode", "DgppId"));
			foreignKey.ReferencedTable = "FirmAddresses";
			foreignKey.ReferencedTableSchema = ErmSchemas.BusinessDirectory;
			foreignKey.Create();
		}

		private static void CreateUniqueIndexOnDepCards(IMigrationContext context)
		{
			// delete duplicates
			context.Connection.ExecuteNonQuery("DELETE FROM Integration.CardRelations WHERE DepCardCode IN (SELECT DepCardCode FROM Integration.CardRelations GROUP BY DepCardCode HAVING COUNT(*) > 1)");

			const string indexName = "UQ_CardRelations_DepCardCode";
			var table = context.Database.Tables["CardRelations", ErmSchemas.Integration];

			if (table.Indexes.Cast<Index>().Any(x => string.Equals(x.Name, indexName, StringComparison.OrdinalIgnoreCase)))
				return;

			// create index
			var index = new Index(table, indexName);
			index.IndexedColumns.Add(new IndexedColumn(index, "DepCardCode"));
			index.IndexKeyType = IndexKeyType.DriUniqueKey;
			index.Create();
		}
	}
}
