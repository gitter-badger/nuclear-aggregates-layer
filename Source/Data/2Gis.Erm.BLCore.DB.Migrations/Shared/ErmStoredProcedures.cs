using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.BLCore.DB.Migrations.Shared
{
    public static class ErmStoredProcedures
    {
        public static readonly SchemaQualifiedObjectName ShowIndexUsages =
            new SchemaQualifiedObjectName(ErmSchemas.Adm, "ShowIndexUsages");

        public static readonly SchemaQualifiedObjectName ReplicateBargain =
            new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateBargain");
        
        public static readonly SchemaQualifiedObjectName ReplicateAccountDetail =
            new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateAccountDetail");
        
        public static readonly SchemaQualifiedObjectName ReplicateOperationType =
            new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateOperationType");

        public static readonly SchemaQualifiedObjectName ReplicateOrder =
            new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateOrder");

        public static readonly SchemaQualifiedObjectName ReplicateAccount =
            new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateAccount");

        public static readonly SchemaQualifiedObjectName ReplicateCurrency =
            new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateCurrency");

        public static readonly SchemaQualifiedObjectName ReplicatePosition =
            new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicatePosition");

        public static readonly SchemaQualifiedObjectName ReplicateClient =
            new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateClient");

        public static readonly SchemaQualifiedObjectName ReplicateContact =
            new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateContact");

        public static readonly SchemaQualifiedObjectName ReplicateDeal =
            new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateDeal");

        public static readonly SchemaQualifiedObjectName ReplicateBranchOffice =
           new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateBranchOffice");

        public static readonly SchemaQualifiedObjectName ReplicateBranchOfficeOrganizationUnit =
           new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateBranchOfficeOrganizationUnit");

        public static readonly SchemaQualifiedObjectName ReplicateLegalPerson =
           new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateLegalPerson");

        public static readonly SchemaQualifiedObjectName ReplicateLimit =
           new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateLimit");

        public static readonly SchemaQualifiedObjectName ReplicateOrderPosition =
           new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateOrderPosition");

        public static readonly SchemaQualifiedObjectName ReplicateOrganizationUnit =
           new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateOrganizationUnit");

        public static readonly SchemaQualifiedObjectName ReplicateCategory =
           new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "ReplicateCategory");

        public static readonly SchemaQualifiedObjectName ReplicateFirm =
           new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "ReplicateFirm");

        public static readonly SchemaQualifiedObjectName ReplicateFirmAddress =
           new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "ReplicateFirmAddress");

        public static readonly SchemaQualifiedObjectName ReplicateTerritory =
           new SchemaQualifiedObjectName(ErmSchemas.BusinessDirectory, "ReplicateTerritory");

        public static readonly SchemaQualifiedObjectName ReplicateUser =
           new SchemaQualifiedObjectName(ErmSchemas.Security, "ReplicateUser");

        public static readonly SchemaQualifiedObjectName ReplicateUserTerritory =
           new SchemaQualifiedObjectName(ErmSchemas.Security, "ReplicateUserTerritory");

        public static readonly SchemaQualifiedObjectName CheckUserParentnessRecursion =
            new SchemaQualifiedObjectName(ErmSchemas.Security, "CheckUserParentnessRecursion");

        public static readonly SchemaQualifiedObjectName ReplicateEverything =
            new SchemaQualifiedObjectName(ErmSchemas.Shared, "ReplicateEverything");

        public static readonly SchemaQualifiedObjectName CreateExportSession =
            new SchemaQualifiedObjectName(ErmSchemas.Integration, "CreateExportSession");

        public static readonly SchemaQualifiedObjectName GetNonExportedOrdersCount =
            new SchemaQualifiedObjectName(ErmSchemas.Integration, "GetNonExportedOrdersCount");

        public static readonly SchemaQualifiedObjectName ReplicateEntitiesToCrm =
            new SchemaQualifiedObjectName(ErmSchemas.Shared, "ReplicateEntitiesToCrm");
    }
}
