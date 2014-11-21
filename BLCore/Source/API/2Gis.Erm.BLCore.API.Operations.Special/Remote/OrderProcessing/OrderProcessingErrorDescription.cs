using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.OrderProcessing
{
    [DataContract(Namespace = ServiceNamespaces.FinancialOperations.FinancialOperations201310)]
    public class  OrderProcessingErrorDescription
    {
        public OrderProcessingErrorDescription(string message)
        {
            Message = message;
        }

        [DataMember]
        public string Message { get; private set; }
    }
}
