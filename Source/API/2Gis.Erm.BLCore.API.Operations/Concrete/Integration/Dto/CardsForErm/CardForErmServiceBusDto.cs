using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm
{
    [ServiceBusObjectDescription("CardForERM")]
    public class CardForErmServiceBusDto : IServiceBusDto<FlowCardsForErm>
    {
        public long Code { get; set; }
        public CardType Type { get; set; }
        public IEnumerable<ContactDto> Contacts { get; set; }
        public IEnumerable<ImportCategoryFirmAddressDto> Rubrics { get; set; }

        public CardAddressDto Address { get; set; }
        public ScheduleDto Schedule { get; set; }
        public PaymentDto Payment { get; set; }
        public long FirmCode { get; set; }
        public int BranchCode { get; set; }
        public bool IsActive { get; set; }
        public bool ClosedForAscertainment { get; set; }
        public bool IsLinked { get; set; }
        public bool IsDeleted { get; set; }
    }
}