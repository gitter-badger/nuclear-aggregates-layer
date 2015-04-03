using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    // TODO {y.baranihin, 03.04.2015}: избавиться
    public interface IDeniedPositionsDuplicatesCleaner : IInvariantSafeCrosscuttingService
    {
        IEnumerable<DeniedPosition> Distinct(IEnumerable<DeniedPosition> deniedPositions);
        void VerifyForDuplicates(IEnumerable<DeniedPosition> deniedPositions);
    }
}
