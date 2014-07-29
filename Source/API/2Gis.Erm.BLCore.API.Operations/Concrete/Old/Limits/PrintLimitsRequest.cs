using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits
{
    public class PrintLimitsRequest : Request
    {
        public IEnumerable<long> LimitIds { get; set; }
    }
}