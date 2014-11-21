using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    [DataContract]
    public class CancelledOrderRequestStateDescription : IOrderRequestStateDescription
    {
        private const OrderProcessingRequestState state = OrderProcessingRequestState.Cancelled;

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
