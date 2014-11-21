using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositionAdvertisementValidation
{
    // TODO {d.ivanov, 16.12.2013}: 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations\Concrete\OrderPositionAdvertisementValidation
    public interface IValidateOrderPositionAdvertisementsService : IOperation<ValidateOrderPositionAdvertisementsIdentity>
    {
        IReadOnlyCollection<OrderPositionAdvertisementValidationError> Validate(long orderPositionId, IEnumerable<AdvertisementDescriptor> advertisements);
    }
}
