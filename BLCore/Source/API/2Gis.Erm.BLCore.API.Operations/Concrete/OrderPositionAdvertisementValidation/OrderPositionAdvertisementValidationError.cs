using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositionAdvertisementValidation
{
    public sealed class OrderPositionAdvertisementValidationError
    {
        public string ErrorMessage { get; set; }
        public OrderPositionAdvertisementValidationRule Rule { get; set; }
        public AdvertisementDescriptor Advertisement { get; set; }
    }
}
