using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements
{
    public interface IChangeAdvertisementElementStatusStrategiesFactory
    {
        IEnumerable<IAdvertisementElementStatusChangingStrategy> EvaluateProcessingStrategies(AdvertisementElementStatusValue currentStatus,
                                                                                              AdvertisementElementStatusValue newStatus);
    }
}
