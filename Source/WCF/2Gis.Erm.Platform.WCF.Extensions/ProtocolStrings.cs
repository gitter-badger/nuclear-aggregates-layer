namespace DoubleGis.Erm.Platform.WCF.Extensions
{
    internal sealed class ProtocolStrings
    {
        public static class ContractNames
        {
            public const string DiscoveryManagedContractName = "DiscoveryProxy";
            public const string DiscoveryAdhocContractName = "TargetService";
        }

        public static class VersionApril2005
        {
            public const string Namespace = "http://schemas.xmlsoap.org/ws/2005/04/discovery";
        }

        public static class VersionCd1
        {
            public const string Namespace = "http://docs.oasis-open.org/ws-dd/ns/discovery/2008/09";
        }

        public static class Version11
        {
            public const string Namespace = "http://docs.oasis-open.org/ws-dd/ns/discovery/2009/01";
        }

        public static class SchemaNames
        {
            public const string ProbeMatchElement = "ProbeMatch";
            public const string TypesElement = "Types";
        }
    }
}
