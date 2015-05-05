using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201504101656, "ERM-6103. Добавляем таблицу Security.TelephonyAddresses, связываем с Security.UserProfiles и заполняем начальными данными", "a.pashkin")]
    public class Mirgation201504101656 : TransactedMigration
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

            var userProfiles = context.Database.GetTable(ErmTableNames.UserProfiles);

            userProfiles.Columns.Add(new Column(userProfiles, "TelephonyAddressId", DataType.BigInt) { Nullable = true });
            userProfiles.Alter();
            userProfiles.CreateForeignKey("TelephonyAddressId", ErmTableNames.TelephonyAddresses, "Id");

            var telephonyAddresses = context.Database.GetTable(ErmTableNames.TelephonyAddresses);

            var insterQuery = string.Format(
                "INSERT INTO [{0}].[{1}] (Id, Name, Address) Values ({2}, '{3}', '{4}')",
                telephonyAddresses.Schema,
                telephonyAddresses.Name,
                NovosibirskTelephonyId,
                "Новосибирск",
                NovosibirskTelephonyAddress);
            context.Connection.ExecuteNonQuery(insterQuery);

            var updateQuery =
                string.Format(
                    "UPDATE [Security].[UserProfiles] " + 
                    "SET TelephonyAddressId = {0} " + 
                    "FROM [Security].[UserProfiles] profiles " + 
                    "JOIN [Security].[Users] users " + 
                    "ON (profiles.UserId = users.Id) " + 
                    "JOIN [Security].[Departments] departments " + 
                    "ON (users.DepartmentId = departments.Id) "
                    + "WHERE departments.Name LIKE '{1}' OR departments.Name = '{2}'",
                    NovosibirskTelephonyId,
                    Match2GisName,
                    MatchNskPattern);
            context.Connection.ExecuteNonQuery(updateQuery);
        }
    }
}
