using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards
{
    [ServiceBusObjectDescription("Reference", ServiceBusObjectProcessingOrder.First)]
    public sealed class ReferenceServiceBusDto : IServiceBusDto<FlowCards>
    {
        public string Code { get; set; }
    }
}