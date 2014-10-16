using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(24094, "Актуализируем скрытые профили юр. лиц", "y.baranihin")]
    public class Migration24094 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(@"update lpp set IsDeleted = 1
  FROM [Billing].[LegalPersonProfiles] lpp 
  join [Billing].[LegalPersons] lp on lpp.LegalPersonId = lp.Id 
  where lpp.IsDeleted = 0 And lpp.IsActive = 0 and lp.IsActive = 1");
        }
    }
}