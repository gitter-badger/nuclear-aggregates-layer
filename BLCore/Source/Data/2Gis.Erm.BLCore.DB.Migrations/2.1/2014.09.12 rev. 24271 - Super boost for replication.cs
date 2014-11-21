using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(24271, "Ускорение хранимок репликации #2", "a.tukaev")]
    public class Migration24271 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateAccount_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateAccountDetail_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateBargain_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateBranchOffice_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateBranchOfficeOrganizationUnit_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateClient_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateContact_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateContacts_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateCurrency_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateDeal_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateLegalPerson_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateLimit_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateOperationType_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateOrder_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateOrderPosition_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateOrderProcessingRequest_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateOrganizationUnit_24271);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicatePosition_24271);
            context.Database.ExecuteNonQuery(Resources._BusinessDirectory___ReplicateCategory_24271);
            context.Database.ExecuteNonQuery(Resources._BusinessDirectory___ReplicateFirm_24271);
            context.Database.ExecuteNonQuery(Resources._BusinessDirectory___ReplicateFirms_24271);
            context.Database.ExecuteNonQuery(Resources._BusinessDirectory___ReplicateFirmAddress_24271);
            context.Database.ExecuteNonQuery(Resources._BusinessDirectory___ReplicateFirmAddresses_24271);
            context.Database.ExecuteNonQuery(Resources._BusinessDirectory___ReplicateTerritory_24271);
            context.Database.ExecuteNonQuery(Resources._BusinessDirectory___ReplicateTerritories_24271);
            context.Database.ExecuteNonQuery(Resources._Security___ReplicateUser_24271);
            context.Database.ExecuteNonQuery(Resources._Security___ReplicateUserTerritory_24271);
        }
    }
}