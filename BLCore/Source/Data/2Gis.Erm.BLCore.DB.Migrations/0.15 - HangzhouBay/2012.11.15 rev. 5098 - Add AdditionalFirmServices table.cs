using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5098, "Добавляем таблицы AdditionalFirmServices и FirmAddressServices")]
    public sealed class Migration5098 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddTable1(context);
            AddTable2(context);
        }

        private static void AddTable1(IMigrationContext context)
        {
            var table = context.Database.Tables["AdditionalFirmServices", ErmSchemas.Integration];
            if (table != null)
                return;

            table = new Table(context.Database, "AdditionalFirmServices", ErmSchemas.Integration);

            var pkIndex = new Index(table, "PK_AdditionalFirmServices");
            pkIndex.IndexedColumns.Add(new IndexedColumn(pkIndex, "Id"));
            pkIndex.IndexKeyType = IndexKeyType.DriPrimaryKey;
            table.Indexes.Add(pkIndex);

            var id = new Column(table, "Id", DataType.Int) {Nullable = false};
            table.Columns.Add(id);

            var serviceCode = new Column(table, "ServiceCode", DataType.NVarChar(200)) { Nullable = false };
            table.Columns.Add(serviceCode);

            var isManaged = new Column(table, "IsManaged", DataType.Bit) { Nullable = false };
            isManaged.AddDefaultConstraint("DF_AdditionalFirmServices_IsManaged").Text = "0";
            table.Columns.Add(isManaged);

            table.Create();
        }

        private static void AddTable2(IMigrationContext context)
        {
            var table = context.Database.Tables["FirmAddressServices", ErmSchemas.BusinessDirectory];
            if (table != null)
                return;

            table = new Table(context.Database, "FirmAddressServices", ErmSchemas.BusinessDirectory);

            var pkIndex = new Index(table, "PK_FirmAddressServices");
            pkIndex.IndexedColumns.Add(new IndexedColumn(pkIndex, "Id"));
            pkIndex.IndexKeyType = IndexKeyType.DriPrimaryKey;
            table.Indexes.Add(pkIndex);

            var id = new Column(table, "Id", DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 };
            table.Columns.Add(id);

            var firmAddressId = new Column(table, "FirmAddressId", DataType.Int) { Nullable = false };
            table.Columns.Add(firmAddressId);

            var fkFirmAddressId = new ForeignKey(table, "FK_FirmAddressServices_FirmAddresses");
            fkFirmAddressId.Columns.Add(new ForeignKeyColumn(fkFirmAddressId, "FirmAddressId", "Id"));
            fkFirmAddressId.ReferencedTable = "FirmAddresses";
            fkFirmAddressId.ReferencedTableSchema = ErmSchemas.BusinessDirectory;
            table.ForeignKeys.Add(fkFirmAddressId);

            var serviceId = new Column(table, "ServiceId", DataType.Int) { Nullable = false };
            table.Columns.Add(serviceId);

            var fkServiceId = new ForeignKey(table, "FK_FirmAddressServices_AdditionalFirmServices");
            fkServiceId.Columns.Add(new ForeignKeyColumn(fkServiceId, "ServiceId", "Id"));
            fkServiceId.ReferencedTable = "AdditionalFirmServices";
            fkServiceId.ReferencedTableSchema = ErmSchemas.Integration;
            table.ForeignKeys.Add(fkServiceId);

            var isManaged = new Column(table, "DisplayService", DataType.Bit) { Nullable = false };
            isManaged.AddDefaultConstraint("DF_FirmAddressServices_DisplayService").Text = "0";
            table.Columns.Add(isManaged);

            table.Create();
        }
    }
}