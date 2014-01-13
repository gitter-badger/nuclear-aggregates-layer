using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.DTOs;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositionAdvertisementValidation
{
    // TODO {d.ivanov, 16.12.2013}: 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations\Concrete\OrderPositionAdvertisementValidation
    public interface IAdvertisementValidationRule
    {
        void Validate(AdvertisementDescriptor advertisements, ICollection<OrderPositionAdvertisementValidationError> errors);
    }
}
