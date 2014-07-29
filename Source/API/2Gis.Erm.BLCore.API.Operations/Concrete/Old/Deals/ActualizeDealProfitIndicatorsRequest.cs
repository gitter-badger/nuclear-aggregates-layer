using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals
// ReSharper restore CheckNamespace
{
    public sealed class ActualizeDealProfitIndicatorsRequest : Request
    {
        public IEnumerable<long> DealIds { get; set; }
    }
}