using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public interface ISymmetricDeniedPositionsVerifier : IInvariantSafeCrosscuttingService
    {
        void VerifyForSymmetryWithinCollection(IEnumerable<DeniedPosition> deniedPositions);
    }
}
