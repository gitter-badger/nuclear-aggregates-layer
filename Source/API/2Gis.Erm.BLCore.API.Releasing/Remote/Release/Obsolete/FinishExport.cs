using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Releasing.Remote.Release.Obsolete
{
    [DataContract(Namespace = ServiceNamespaces.Releasing.BackwardCompatibility201308)]
    public class FinishExportRequest : Request
    {
        [DataMember]
        public long ReleaseInfoId { get; set; }

        [DataMember]
        public bool IsSuccessed { get; set; }

        [DataMember]
        public ValidationResult[] ValidationErrors { get; set; }
    }

    [DataContract(Namespace = ServiceNamespaces.Releasing.BackwardCompatibility201308)]
    public class FinishExportResponse : Response
    {
    }
}