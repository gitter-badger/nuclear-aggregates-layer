using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2307, "Создание таблицы CityPhoneZone - справочник телефонных кодов из InfoRussia")]
    public class Migration2307 : TransactedMigration
    {
        private readonly SchemaQualifiedObjectName _cityPhoneZone = new SchemaQualifiedObjectName(ErmSchemas.Integration, "CityPhoneZone");

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.Database.Tables.Contains(_cityPhoneZone.Name, _cityPhoneZone.Schema))
            {
                return;
            }

            var table = new Table(context.Database, _cityPhoneZone.Name, _cityPhoneZone.Schema);

            table.Columns.Add(new Column(table, "Code", DataType.Int) { Nullable = false });
            table.Columns.Add(new Column(table, "Name", DataType.NVarChar(50)) { Nullable = false });
            table.Columns.Add(new Column(table, "CityCode", DataType.BigInt) { Nullable = false });
            table.Columns.Add(new Column(table, "IsDefault", DataType.Bit) { Nullable = true });
            table.Columns.Add(new Column(table, "IsDeleted", DataType.Bit) { Nullable = true });

            table.Create();
            table.CreatePrimaryKey("Code");
        }
    }
}
