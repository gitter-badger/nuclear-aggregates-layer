using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.FirmWorkStatus
{
    [DataContract(Namespace = ServiceNamespaces.FinancialOperations.FirmWorkStatus201505)]
    public class FirmWorkStatus
    {
        [DataMember]
        public bool IsInReserve { get; set; }
    }
}