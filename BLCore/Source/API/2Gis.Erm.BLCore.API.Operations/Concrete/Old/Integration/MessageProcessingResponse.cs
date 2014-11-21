using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration
{
    public abstract class MessageProcessingResponse : Response
    {
        public IEnumerable<string> Messages { get; set; }
    }
}
