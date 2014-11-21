using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card.OrderPosition.DTOs
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
