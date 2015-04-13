using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201504101650, "ERM-6103. Добавляем поля CanCall,TelephonyAddress в Security.UserProfiles и включаем возможность звонить только маленькой группе людей", "a.pashkin")]
    public class Mirgation201504101650 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var userProfiles = context.Database.GetTable(ErmTableNames.UserProfiles);
            var users = context.Database.GetTable(ErmTableNames.Users);
            const string CanCallColumn = "CanCall";
            const string TelephonyAddressColumn = "TelephonyAddress";
            const int CanCallDefaultValue = 0;
            const int CanCallAcceptedValue = 1;
            const string AllowedTelephonyUsers = "'i.levenets','m.pyshkin','e.kolchina', 't.potapova','e.polischuk', 'kyb'";
            const string NovosibirskTelephonyAddress = "uk-erm-tapi01:1313";

            var newColumns = new[]
                                 {
                                     new InsertedColumnDefinition(11, x => new Column(x, CanCallColumn, DataType.Bit) { Nullable = true }),
                                     new InsertedColumnDefinition(11, x => new Column(x, TelephonyAddressColumn, DataType.NVarChar(50)) { Nullable = true })
                                 };
            EntityCopyHelper.CopyAndInsertColumns(context.Database, userProfiles, newColumns);

            context.Connection.ExecuteNonQuery(string.Format("Update [{0}].[{1}] set [{2}]={3}", userProfiles.Schema, userProfiles.Name, CanCallColumn, CanCallDefaultValue));
            userProfiles = context.Database.GetTable(ErmTableNames.UserProfiles);
            userProfiles.Columns[CanCallColumn].Nullable = false;
            userProfiles.Columns[CanCallColumn].Alter();

            var updateQuery =
                string.Format(
                    "UPDATE profiles SET {0} = {1}, {2} = '{3}' FROM [{4}].[{5}] profiles INNER JOIN [{6}].[{7}]  users ON users.Id = profiles.UserId WHERE Account IN ({8}) ",
                    CanCallColumn,
                    CanCallAcceptedValue,
                    TelephonyAddressColumn,
                    NovosibirskTelephonyAddress,
                    userProfiles.Schema,
                    userProfiles.Name,
                    users.Schema,
                    users.Name,
                    AllowedTelephonyUsers);
            context.Connection.ExecuteNonQuery(updateQuery);
        }
    }
}
