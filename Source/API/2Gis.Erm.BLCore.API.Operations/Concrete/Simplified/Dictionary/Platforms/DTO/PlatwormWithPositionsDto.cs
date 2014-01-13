using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Platforms.DTO
{
    public sealed class PlatwormWithPositionsDto
    {
        public Erm.Platform.Model.Entities.Erm.Platform Platform { get; set; }
        public IEnumerable<Position> Positions { get; set; }
    }
}
