using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201504101650, "ERM-6103. Добавляем таблицу Shared.TelephonyAddresses, связываем с Security.Departments и заполняем начальными данными", "a.pashkin")]
    public class Mirgation201504101650 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {            
            const string NovosibirskTelephonyAddress = "tapi://uk-erm-tapi01:1313";
            const string MatchNskPattern = "%Новосибирск%";
            const string Match2GisName = "2ГИС";
            const long NovosibirskTelephonyId = 615616117641052232;
            if (context.Database.Tables.Contains(ErmTableNames.TelephonyAddresses.Name, ErmTableNames.TelephonyAddresses.Schema))
            {
                return;
            }

            var table = new Table(context.Database, ErmTableNames.TelephonyAddresses.Name, ErmTableNames.TelephonyAddresses.Schema);
            table.Columns.Add(new Column(table, "Id", DataType.BigInt)
            {
                Nullable = false              
            });
            table.Columns.Add(new Column(table, "Name", DataType.NVarChar(50)) { Nullable = false });
            table.Columns.Add(new Column(table, "Address", DataType.NVarChar(50)) { Nullable = false });

            table.Create();
            table.CreatePrimaryKey("Id");            

            var departments = context.Database.GetTable(ErmTableNames.Departments);

            departments.Columns.Add(new Column(departments, "TelephonyAddressId", DataType.BigInt) { Nullable = true });
            departments.Alter();
            departments.CreateForeignKey("TelephonyAddressId", ErmTableNames.TelephonyAddresses, "Id");

            var telephonyAddresses = context.Database.GetTable(ErmTableNames.TelephonyAddresses);

            var insterQuery = string.Format(
                "INSERT INTO [{0}].[{1}] (Id, Name, Address) Values ({2}, '{3}', '{4}')",
                telephonyAddresses.Schema,
                telephonyAddresses.Name,
                NovosibirskTelephonyId,
                "Новосибирск",
                NovosibirskTelephonyAddress);
            context.Connection.ExecuteNonQuery(insterQuery);

            var updateQuery = string.Format(
                "UPDATE [{0}].[{1}] SET TelephonyAddressId = {2} WHERE Name LIKE '{3}' OR Name = '{4}'",
                departments.Schema,
                departments.Name,
                NovosibirskTelephonyId,
                MatchNskPattern,
                Match2GisName);
            context.Connection.ExecuteNonQuery(updateQuery);
        }
    }
}
