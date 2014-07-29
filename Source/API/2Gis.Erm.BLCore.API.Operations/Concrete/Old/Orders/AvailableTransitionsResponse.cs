using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    public sealed class AvailableTransitionsResponse : Response
    {
        public IEnumerable<OrderState> AvailableTransitions { get; private set; }

        public AvailableTransitionsResponse(IEnumerable<OrderState> availableTransitions)
        {
            AvailableTransitions = availableTransitions;
        }
    }
}
