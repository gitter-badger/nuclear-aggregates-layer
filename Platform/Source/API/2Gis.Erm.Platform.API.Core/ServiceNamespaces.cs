using System;

namespace DoubleGis.Erm.Platform.API.Core
{
    public static class ServiceNamespaces
    {
        public const string Prefix = "http://2gis.ru/erm/api";

        public static bool IsErmService(string serviceNamespace)
        {
            return serviceNamespace.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase);
        }

        public static class Common
        {
             public const string Common201309 = "http://2gis.ru/erm/api/common/2013/09";
        }

        public static class BasicOperations
        {
            public const string ActionsHistory201303 = "http://2gis.ru/erm/api/basicoperations/actionshistory/2013/03";
            public const string Activate201303 = "http://2gis.ru/erm/api/basicoperations/activate/2013/03";
            public const string Deactivate201303 = "http://2gis.ru/erm/api/basicoperations/deactivate/2013/03";
            public const string Append201303 = "http://2gis.ru/erm/api/basicoperations/append/2013/03";
            public const string Assign201303 = "http://2gis.ru/erm/api/basicoperations/assign/2013/03";
            public const string CreateOrUpdate201304 = "http://2gis.ru/erm/api/basicoperations/createorupdate/2013/04";
            public const string ChangeClient201303 = "http://2gis.ru/erm/api/basicoperations/changeclient/2013/03";
            public const string ChangeTerritory201303 = "http://2gis.ru/erm/api/basicoperations/changeterritory/2013/03";
            public const string CheckForDebts201303 = "http://2gis.ru/erm/api/basicoperations/checkfordebts/2013/03";
            public const string Delete201303 = "http://2gis.ru/erm/api/basicoperations/delete/2013/03";
            public const string GetDomainEntityDto201306 = "http://2gis.ru/erm/api/basicoperations/getdomainentitydto/2013/06";
            public const string Qualify201303 = "http://2gis.ru/erm/api/basicoperations/qualify/2013/03";
            public const string Disqualify201303 = "http://2gis.ru/erm/api/basicoperations/disqualify/2013/03";
            public const string List201303 = "http://2gis.ru/erm/api/basicoperations/list/2013/03";
            public const string DownloadBinary201307 = "http://2gis.ru/erm/api/basicoperations/downloadbinary/2013/07";
            public const string UploadBinary201307 = "http://2gis.ru/erm/api/basicoperations/uploadbinary/2013/07";
            public const string MsCrm201309 = "http://2gis.ru/erm/api/basicoperations/mscrm/2013/09";
            public const string Cancel201502 = "http://2gis.ru/erm/api/basicoperations/Cancel/2015/02";
            public const string Complete201502 = "http://2gis.ru/erm/api/basicoperations/Complete/2015/02";
            public const string Reopen201502 = "http://2gis.ru/erm/api/basicoperations/Revert/2015/02";
        }

        public static class OrderValidation
        {
            public const string OrderValidation201303 = "http://2gis.ru/erm/api/ordervalidation/2013/03";
        }

        public static class Identity
        {
            public const string Identity201303 = "http://2gis.ru/erm/api/identity/2013/03";
        }

        public static class Metadata
        {
            public const string Metadata201307 = "http://2gis.ru/erm/api/metadata/list/2013/07";
        }

        public static class MoneyDistibution
        {
            public static class Common
            {
                public const string ServiceContract = "http://2gis.ru/erm/api/moneydistribution/2013/07";
                public const string DataContract = ServiceContract + "/data";
            }

            public static class AccountingSystem
            {
                public const string ServiceContract = "http://2gis.ru/erm/api/moneydistribution/accountingsystem/2013/07";
                public const string DataContract = ServiceContract + "/data";
            }

            public static class WithdrawalInfo
            {
                public const string ServiceContract = "http://2gis.ru/erm/api/moneydistribution/withdrawalinfo/2013/07";
                public const string DataContract = ServiceContract + "/data";
            }

            public static class Reports
            {
                public const string ServiceContract = "http://2gis.ru/erm/api/moneydistribution/reports/2013/07";
                public const string DataContract = ServiceContract + "/data";
            }
        }

        public static class FinancialOperations
        {
            public const string FinancialOperations201310 = "http://2gis.ru/erm/api/financialoperations/2013/10";
            public const string FirmInfo201402 = "http://2gis.ru/erm/api/financialoperations/firminfo/2014/02";
        }

        public static class Releasing
        {
            public const string Release201308 = "http://2gis.ru/erm/api/releasing/release/2013/08";
        }

        public static class AdsManagement
        {
            public const string HandleAdsState201407 = "http://2gis.ru/erm/api/adsmanagement/handleadsstate/2014/07";
            public const string ManageTextAds201407 = "http://2gis.ru/erm/api/adsmanagement/managetextads/2014/07";
        }
    }
}