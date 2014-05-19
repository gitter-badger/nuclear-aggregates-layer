using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards
{
    [ServiceBusObjectDescription("Card", ServiceBusObjectProcessingOrder.Third)]
    public sealed class CardServiceBusDto : IServiceBusDto<FlowCards>
    {
        public XElement Content { get; set; }
    }
}