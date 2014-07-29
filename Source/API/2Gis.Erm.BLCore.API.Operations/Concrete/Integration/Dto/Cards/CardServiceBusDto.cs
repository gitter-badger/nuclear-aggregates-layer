using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards
{
    [ServiceBusObjectDescription("Card", ServiceBusObjectProcessingOrder.Third)]
    public abstract class CardServiceBusDto : IServiceBusDto<FlowCards>
    {
    }
}