using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public interface IDeniedPositionsDuplicatesVerifier : IInvariantSafeCrosscuttingService
    {
        void VerifyForDuplicates(long positionId, long positionDeniedId, long priceId, params long[] deniedPositionToExcludeIds);
        void VerifyForDuplicatesWithinCollection(IEnumerable<DeniedPosition> deniedPositions);
    }
}
