using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards
{
    [ServiceBusObjectDescription("ReferenceItem", ServiceBusObjectProcessingOrder.Second)]
    public sealed class ReferenceItemServiceBusDto : IServiceBusDto<FlowCards>
    {
        public int Code { get; set; }
        public string ReferenceCode { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}