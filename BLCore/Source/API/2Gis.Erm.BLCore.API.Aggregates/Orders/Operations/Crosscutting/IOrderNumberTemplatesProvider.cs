using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting
{
    public interface IOrderNumberTemplatesProvider : IInvariantSafeCrosscuttingService
    {
        string GetTemplate(OrderType orderType);
        string GetRegionalTemplate();
    }
}