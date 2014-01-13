using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    [DataContract]
    public class CompletedOrderRequestStateDescription : IOrderRequestStateDescription
    {
        private const OrderProcessingRequestState state = OrderProcessingRequestState.Completed;

        [DataMember]
        public long OrderId { get; set; }

        [DataMember]
        public long RequestId { get; set; }

        [DataMember]
        public string State
        {
            get { return state.ToString(); }
            set { }
        }
    }
}
