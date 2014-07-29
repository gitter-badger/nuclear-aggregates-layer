using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(12781, "Исправление статусов AdvertisementElements")]
    public class Migration12781 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(@"update Billing.AdvertisementElements 
set status = 0
where id in (
SELECT distinct AE.Id
  FROM Billing.AdvertisementElements AE
  join Security.Users U on U.Id = AE.ModifiedBy
  join Security.UserRoles UR on UR.UserId = U.Id

  where AE.IsDeleted = 0
  and AE.Status = 2
  and U.Id not in (select UserId from Security.UserRoles where RoleId in (11, 14)))");
        }
    }
}
