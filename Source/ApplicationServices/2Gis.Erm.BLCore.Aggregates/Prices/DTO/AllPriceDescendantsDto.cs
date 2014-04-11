using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.DTO
{
    public sealed class AllPriceDescendantsDto
    {
        public IEnumerable<PricePosition> PricePositions { get; set; }
        public IDictionary<long, IEnumerable<AssociatedPositionsGroup>> AssociatedPositionsGroupsMapping { get; set; }
        public IDictionary<long, IEnumerable<AssociatedPosition>> AssociatedPositionsMapping { get; set; }
        public IEnumerable<DeniedPosition> DeniedPositions { get; set; }
    }
}