﻿using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.OrderValidation.Settings
{
    public interface IOrderValidationSettings : ISettings
    {
        IReadOnlyDictionary<long, IEnumerable<PricePositionDto.RelatedItemDto>> GetGlobalDeniedPositions(IEnumerable<long> requiredPositionIds);
        IReadOnlyDictionary<long, IEnumerable<PricePositionDto.RelatedItemDto>> GetGlobalAssociatedPositions(IEnumerable<long> requiredPositionIds);
    }
}
