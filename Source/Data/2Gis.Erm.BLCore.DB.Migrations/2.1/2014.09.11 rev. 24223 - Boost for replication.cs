using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(24223, "Ускорение хранимок репликации", "a.tukaev")]
    public class Migration24223 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateAccount_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateAccountDetail_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateBargain_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateBranchOffice_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateBranchOfficeOrganizationUnit_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateClient_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateContact_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateContacts_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateCurrency_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateDeal_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateLegalPerson_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateLimit_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateOperationType_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateOrder_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateOrderPosition_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateOrderProcessingRequest_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateOrganizationUnit_24233);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicatePosition_24233);
            context.Database.ExecuteNonQuery(Resources._BusinessDirectory___ReplicateCategory_24233);
            context.Database.ExecuteNonQuery(Resources._BusinessDirectory___ReplicateFirm_24233);
            context.Database.ExecuteNonQuery(Resources._BusinessDirectory___ReplicateFirmAddress_24233);
            context.Database.ExecuteNonQuery(Resources._BusinessDirectory___ReplicateTerritory_24233);
            context.Database.ExecuteNonQuery(Resources._Security___ReplicateUser_24233);
            context.Database.ExecuteNonQuery(Resources._Security___ReplicateUserTerritory_24233);
        }
    }
}