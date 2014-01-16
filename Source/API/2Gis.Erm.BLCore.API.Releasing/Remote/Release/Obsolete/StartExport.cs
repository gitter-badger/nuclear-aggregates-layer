using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Releasing.Remote.Release.Obsolete
{
    [DataContract(Namespace = ServiceNamespaces.Releasing.BackwardCompatibility201308)]
    public class StartExportRequest : Request
    {
        [DataMember]
        public int OrganizationUnitId { get; set; }

        [DataMember]
        public bool IsTechnical { get; set; }

        [DataMember]
        public bool IgnoreBlockingErrors { get; set; }

        [DataMember]
        public DateTime PeriodStart { get; set; }

        [DataMember]
        public DateTime PeriodEnd { get; set; }
    }

    [DataContract(Namespace = ServiceNamespaces.Releasing.BackwardCompatibility201308)]
    public class StartExportResponse : Response
    {
        [DataMember]
        public long ReleaseInfoId { get; set; }

        [DataMember]
        public ValidationResult[] ValidationResults { get; set; }
    }

    [DataContract(Namespace = ServiceNamespaces.Releasing.BackwardCompatibility201308)]
    public class ValidationResult
    {
        [DataMember]
        public long? OrderId { get; set; }

        [DataMember]
        public string OrderNumber { get; set; }

        [DataMember]
        public bool IsBlocking { get; set; }

        [DataMember]
        public string RuleCode { get; set; }

        [DataMember]
        public string Message { get; set; }

        public override string ToString()
        {
            return string.Format("OrderId: {0}, IsBlocking: {1}, RuleCode: [{2}], Message: {3}", OrderId, IsBlocking, RuleCode, Message);
        }
    }
}