using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(23953, "Деактивация профилей дективированных пользователй", "a.rechkalov")]
    public class Migration23953 : TransactedMigration
    {
        private const string Command = "UPDATE Security.UserProfiles " +
                                       "SET UserProfiles.IsActive = Users.IsActive " +
                                       "FROM Security.Users inner join Security.UserProfiles on Users.Id = UserProfiles.UserId";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Command);
        }
    }
}
