using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.OrderValidation;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules.AssociatedAndDenied
{
    public interface IPriceConfigurationService
    {
        void LoadConfiguration(IEnumerable<long> requiredPriceIds, IEnumerable<long> requiredPositionIds, IList<OrderValidationMessage> messages);
        PriceConfigurationStorage GetPriceConfigurationStorage(long priceId, long positionId);
    }
}
