using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient
{
    [Serializable]
    public class ChangeClientOfLinkedDealException : BusinessLogicException
    {
        public ChangeClientOfLinkedDealException(string message)
            : base(message)
        {
        }

        protected ChangeClientOfLinkedDealException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
