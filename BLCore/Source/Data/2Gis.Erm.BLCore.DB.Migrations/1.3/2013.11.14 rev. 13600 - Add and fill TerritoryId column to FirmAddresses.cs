using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    [Migration(13600, "Добавляем TerritoryId в FirmAddresses, заполняем на основании таблицы Buildings", "a.tukaev")]
    public class Migration13600 : TransactedMigration
    {
        private const string FillTerritoryId = @"update fa set fa.[TerritoryId] = b.[TerritoryId]
	                                                from [BusinessDirectory].[FirmAddresses] fa
                                                    join [Integration].[Buildings] b on b.[Code] = fa.[BuildingCode]";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var firmAddressesTable = context.Database.GetTable(ErmTableNames.FirmAddresses);
            var territoriesTable = context.Database.GetTable(ErmTableNames.Territories);

            if (firmAddressesTable == null || territoriesTable == null)
            {
                return;
            }

            var newColumns = new[]
                {
                    new InsertedColumnDefinition(3, x => new Column(x, "TerritoryId", DataType.BigInt) { Nullable = true })
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, firmAddressesTable, newColumns);
            firmAddressesTable = context.Database.GetTable(ErmTableNames.FirmAddresses);


            context.Database.ExecuteNonQuery(FillTerritoryId);

            CreateFkAndIndex(firmAddressesTable, territoriesTable);
        }

        private static void CreateFkAndIndex(Table firmAddressesTable, Table territoriesTable)
        {
            var fk = new ForeignKey(firmAddressesTable, "FK_FirmAddresses_Territories")
                {
                    ReferencedTable = territoriesTable.Name,
                    ReferencedTableSchema = territoriesTable.Schema
                };

            fk.Columns.Add(new ForeignKeyColumn(fk, "TerritoryId", "Id"));
            fk.Create();

            var idx = new Index(firmAddressesTable, "IX_FirmAddresses_TerritoryId");
            idx.IndexedColumns.Add(new IndexedColumn(idx, "TerritoryId"));
            idx.Create();
        }
    }
}