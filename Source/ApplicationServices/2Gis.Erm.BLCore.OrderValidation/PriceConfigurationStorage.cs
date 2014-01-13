using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Prices;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    public class PriceConfigurationStorage
    {
        public static readonly PriceConfigurationStorage Empty = new PriceConfigurationStorage(new List<PricePositionDto.RelatedItemDto>(),
                                                                                               new List<PricePositionDto.RelatedItemDto>());

        public PriceConfigurationStorage(IEnumerable<PricePositionDto.RelatedItemDto> principalPositions,
                                         IEnumerable<PricePositionDto.RelatedItemDto> deniedPositions)
        {
            PrincipalPositions = principalPositions.ToList();
            DeniedPositions = deniedPositions.ToList();
        }

        public List<PricePositionDto.RelatedItemDto> PrincipalPositions { get; private set; }
        public List<PricePositionDto.RelatedItemDto> DeniedPositions { get; private set; }
    }
}
