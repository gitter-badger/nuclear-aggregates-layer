using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.BLCore.DB.Migrations.Shared
{
    public static class CrmTableNames
    {
        // ReSharper disable InconsistentNaming

        public static readonly SchemaQualifiedObjectName OpportunityExtensionBase = 
            new SchemaQualifiedObjectName("dbo", "OpportunityExtensionBase");

        public static readonly SchemaQualifiedObjectName AppointmentExtensionBase = 
            new SchemaQualifiedObjectName("dbo", "AppointmentExtensionBase");

        public static readonly SchemaQualifiedObjectName PhoneCallExtensionBase = 
            new SchemaQualifiedObjectName("dbo", "PhoneCallExtensionBase");

        public static readonly SchemaQualifiedObjectName SavedQueryBase =
            new SchemaQualifiedObjectName("dbo", "SavedQueryBase");

        public static readonly SchemaQualifiedObjectName SystemUserBase =
            new SchemaQualifiedObjectName("dbo", "SystemUserBase");

        public static readonly SchemaQualifiedObjectName Dg_orderExtensionBase =
            new SchemaQualifiedObjectName("dbo", "Dg_orderExtensionBase");

        public static readonly SchemaQualifiedObjectName Dg_bargainExtensionBase =
            new SchemaQualifiedObjectName("dbo", "Dg_bargainExtensionBase");

        public static readonly SchemaQualifiedObjectName ContactBase =
            new SchemaQualifiedObjectName("dbo", "ContactBase");

        // ReSharper restore InconsistentNaming
    }
}
