using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201504101653, "ERM-6103. Добавляем TelephonyAddress в Security.Department и заполняем это поле только для Новосибирска и УК", "a.pashkin")]
    public class Mirgation201504101653 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var departments = context.Database.GetTable(ErmTableNames.Departments);
            const string TelephonyAddressColumn = "TelephonyAddress";            
            const string NovosibirskTelephonyAddress = "tapi://uk-erm-tapi01:1313";
            const string MatchNskPattern = "%Новосибирск%";
            const string Match2GisName = "2ГИС";

            departments.CreateField(TelephonyAddressColumn, DataType.VarChar(50), true);
            departments.Alter();         

            var updateQuery =
                string.Format(
                    "UPDATE [{2}].[{3}] SET {0} = '{1}' WHERE Name LIKE '{4}' OR Name = '{5}' ", 
                    TelephonyAddressColumn, 
                    NovosibirskTelephonyAddress, 
                    departments.Schema, 
                    departments.Name, 
                    MatchNskPattern, 
                    Match2GisName);
            context.Connection.ExecuteNonQuery(updateQuery);
        }
    }
}
