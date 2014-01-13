using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits
// ReSharper restore CheckNamespace
{
    public class PrintLimitsRequest : Request
    {
        public IEnumerable<long> LimitIds { get; set; }
    }
}