using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.DTO
{
    public class AllPricePositionDescendantsDto
    {
        public IEnumerable<AssociatedPositionsGroup> AssociatedPositionsGroups { get; set; }
        public IDictionary<long, IEnumerable<AssociatedPosition>> AssociatedPositionsMapping { get; set; }
        public IEnumerable<DeniedPosition> DeniedPositions { get; set; } 
    }
}