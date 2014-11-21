using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto
{
    public sealed class CategoryWithPositionsDto
    {
        public PositionCategory PositionCategory { get; set; }
        public IEnumerable<Position> Positions { get; set; }
    }
}