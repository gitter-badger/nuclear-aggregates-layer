using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card.OrderPosition;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card.OrderPosition.DTOs;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability
// ReSharper restore CheckNamespace
{
    public static partial class DesignTimeDataContainer
    {
        public static class Cards
        {
            public static object OrderPositionAdvertisementViewModel
            {
                get
                {
                    var orderPositionDto = DebugDataProvider.GetOrderPositionDto();
                    return orderPositionDto.Advertisements.Select(dto => new OrderPositionAdvertisementViewModel(dto, new List<OrderPositionAdvertisementDto>()));
                }
            }

        }
    }
}
