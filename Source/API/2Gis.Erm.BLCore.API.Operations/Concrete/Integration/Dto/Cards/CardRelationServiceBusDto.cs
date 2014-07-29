using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards
{
    [ServiceBusObjectDescription("CardRelation", ServiceBusObjectProcessingOrder.Fourth)]
    public sealed class CardRelationServiceBusDto : IServiceBusDto<FlowCards>
    {
        public long Code { get; set; }
        public long PointOfServiceCardCode { get; set; }
        public long DepartmentCardCode { get; set; }
        public int DepartmentCardSortingPosition { get; set; }
        public bool IsDeleted { get; set; }
    }
}