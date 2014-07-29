using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    [DataContract]
    public sealed class CanCreateOrderPositionForOrderResponse : Response
    {
        [DataMember]
        public bool CanCreate
        {
            get { return string.IsNullOrEmpty(Message); }
        }
        [DataMember]
        public string Message { get; set; }
    }
}
