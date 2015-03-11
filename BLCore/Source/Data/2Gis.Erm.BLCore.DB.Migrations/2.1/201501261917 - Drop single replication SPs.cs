using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201501261917, "Drop single replication SPs", "a.tukaev")]
    public class Migration201501261917 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var spsToDelete = new[]
                                  {
                                      Tuple.Create("Billing", "ReplicateClient"), 
                                      Tuple.Create("Billing", "ReplicateContact"), 
                                      Tuple.Create("Billing", "ReplicateLegalPerson"), 
                                      Tuple.Create("Billing", "ReplicateAccount"), 
                                      Tuple.Create("Billing", "ReplicateAccountDetail"), 
                                      Tuple.Create("Billing", "ReplicateDeal"), 
                                      Tuple.Create("Billing", "ReplicateOrder"), 
                                      Tuple.Create("Billing", "ReplicateOrderPosition"), 
                                      Tuple.Create("Billing", "ReplicateBargain"), 
                                      Tuple.Create("BusinessDirectory", "ReplicateFirm"), 
                                      Tuple.Create("BusinessDirectory", "ReplicateFirmAddress"), 
                                      Tuple.Create("Activity", "ReplicateAppointment"), 
                                      Tuple.Create("Activity", "ReplicateLetter"), 
                                      Tuple.Create("Activity", "ReplicatePhonecall"), 
                                      Tuple.Create("Activity", "ReplicateTask")
                                  };

            foreach (var spToDelete in spsToDelete)
            {
                var sp = context.Database.StoredProcedures[spToDelete.Item2, spToDelete.Item1];
                if (sp != null)
                {
                    sp.Drop();
                }
            }
        }
    }
}