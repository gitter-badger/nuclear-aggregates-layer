using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public interface IVerifyDeniedPositionsForDuplicatesOperationService : IOperation<VerifyDeniedPositionsForDuplicatesIdentity>
    {
        void Verify(long positionId, long positionDeniedId, long priceId, params long[] deniedPositionToExcludeIds);
        void VerifyWithinCollection(IEnumerable<DeniedPosition> deniedPositions);
    }
}
