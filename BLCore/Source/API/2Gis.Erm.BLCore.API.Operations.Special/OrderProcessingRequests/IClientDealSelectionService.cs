using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // TODO {v.lapeev, 05.12.2013}: Методы этого сервиса в 2+ должны уехать в соответсвующие ReadModel. Сервис в 2+ не нужен.
    // DONE {y.baranihin, 05.12.2013}: действительно
    public interface IClientDealSelectionService : ISimplifiedModelConsumer
    {
        // TODO {d.ivanov, 05.12.2013}: Read-model
        IEnumerable<Deal> GetOpenedDeals(long clientId);

        // TODO {d.ivanov, 05.12.2013}: Read-model
        Deal GetDealForOrder(long orderId);

        // TODO {d.ivanov, 05.12.2013}: Read-model
        long? GetOrderClientId(long orderId);

        // TODO {d.ivanov, 05.12.2013}: Read-model
        long? GetMainFirmId(long clientId);
    }
}