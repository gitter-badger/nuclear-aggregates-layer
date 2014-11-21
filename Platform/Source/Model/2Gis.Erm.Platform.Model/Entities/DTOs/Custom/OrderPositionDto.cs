using System.Collections.Generic;

using DoubleGis.Erm.Model.Entities.Erm;
using DoubleGis.Erm.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Core.Dto.DomainEntity.Custom
{
    public class OrderPositionDto// : IDomainEntityDto<OrderPosition>
    {
        public long Id { get; set; }

        public string PricePosition { get; set; }
        public string Platform { get; set; }
        public OrderPositionPriceDto Price { get; set; }
        public IEnumerable<OrderPositionAdvertisementDto> Advertisements { get; set; }
    }
}
