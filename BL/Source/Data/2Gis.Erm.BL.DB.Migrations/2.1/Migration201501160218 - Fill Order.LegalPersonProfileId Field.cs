using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(201501160218, "Fill Order.LegalPersonProfileId field where possible", "a.rechkalov")]
    public class Migration201501160218 : TransactedMigration
    {
        private const string UpdateStatement =
            "update Billing.Orders " +
            "set Orders.LegalPersonProfileId = (select LegalPersonProfiles.Id " +
            "                                   from Billing.LegalPersons " +
            "                                      inner join Billing.LegalPersonProfiles on LegalPersons.Id = LegalPersonProfiles.LegalPersonId " +
            "                                   where LegalPersons.IsActive = 1 " +
            "                                      and LegalPersons.IsDeleted = 0 " +
            "                                      and LegalPersons.Id = Orders.LegalPersonId " +
            "                                      and LegalPersonProfiles.IsActive = 1 " +
            "                                      and LegalPersonProfiles.IsDeleted = 0) " +
            "where Orders.LegalPersonProfileId is null " +
            "    and Orders.IsActive = 1 " +
            "    and Orders.IsDeleted = 0 " +
            "    and Orders.WorkflowStepId <> 6 " + // Кроме архивных
            "    and (select count(*) " +
            "         from Billing.LegalPersons " +
            "            inner join Billing.LegalPersonProfiles on LegalPersons.Id = LegalPersonProfiles.LegalPersonId " +
            "         where LegalPersons.IsActive = 1 " +
            "            and LegalPersons.IsDeleted = 0 " +
            "            and LegalPersons.Id = Orders.LegalPersonId " +
            "            and LegalPersonProfiles.IsActive = 1 " +
            "            and LegalPersonProfiles.IsDeleted = 0) = 1";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(UpdateStatement);
        }
    }
}