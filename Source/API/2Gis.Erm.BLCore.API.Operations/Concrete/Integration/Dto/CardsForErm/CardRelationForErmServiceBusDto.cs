using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm
{
    [ServiceBusObjectDescription("CardRelationForERM")]
    public class CardRelationForErmServiceBusDto : IServiceBusDto<FlowCardsForErm>
    {
        public long Code { get; set; }
        public long Card1Code { get; set; }
        public long Card2Code { get; set; }
        public long BranchCode { get; set; }
        public int OrderNo { get; set; }
        public bool IsDeleted { get; set; }
    }
}