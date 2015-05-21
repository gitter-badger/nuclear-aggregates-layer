using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201504101668, "ERM-6103. Добавляем столбец TelephonyAddress в Security.UserProfiles и заполняем начальными данными", "a.pashkin")]
    public class Mirgation201504101668 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {            
            const string NovosibirskTelephonyUnit = "tapi://uk-erm-tapi01:1313";
            const string MatchNskPattern = "%Новосибирск%";
            const string Match2GisName = "2ГИС";

            var userProfiles = context.Database.GetTable(ErmTableNames.UserProfiles);

            userProfiles.Columns.Add(new Column(userProfiles, "TelephonyAddress", DataType.NVarChar(50)) { Nullable = true });
            userProfiles.Alter();          

            var updateQuery =
                string.Format(
                    "UPDATE [Security].[UserProfiles] " + 
                    "SET TelephonyAddress = '{0}' " + 
                    "FROM [Security].[UserProfiles] profiles " + 
                    "JOIN [Security].[Users] users " + 
                    "ON (profiles.UserId = users.Id) " + 
                    "JOIN [Security].[Departments] departments " + 
                    "ON (users.DepartmentId = departments.Id) " + 
                    "WHERE departments.Name LIKE '{1}' OR departments.Name = '{2}'",
                    NovosibirskTelephonyUnit,
                    MatchNskPattern,
                    Match2GisName);
            context.Connection.ExecuteNonQuery(updateQuery);
        }
    }
}
