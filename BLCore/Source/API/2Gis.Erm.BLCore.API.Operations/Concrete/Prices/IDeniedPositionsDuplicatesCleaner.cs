using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    // Не самое удочное название, но ничего точнее в голову не пришло
    public interface IDeniedPositionsDuplicatesCleaner : IInvariantSafeCrosscuttingService
    {
        IEnumerable<DeniedPosition> Distinct(IEnumerable<DeniedPosition> deniedPositions);
        void VerifyForDuplicates(IEnumerable<DeniedPosition> deniedPositions);
    }
}
