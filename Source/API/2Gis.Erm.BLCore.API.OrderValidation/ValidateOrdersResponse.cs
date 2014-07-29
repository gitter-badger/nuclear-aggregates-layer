using System.Collections.Generic;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    [DataContract]
    public sealed class ValidateOrdersResponse : Response
    {
        public ValidateOrdersResponse(IEnumerable<OrderValidationMessage> messages)
        {
            Messages = messages;
            OrderCount = null;
        }

        [DataMember]
        public IEnumerable<OrderValidationMessage> Messages { get; private set; }
        [DataMember]
        public int? OrderCount { get; private set; }
    }
}
