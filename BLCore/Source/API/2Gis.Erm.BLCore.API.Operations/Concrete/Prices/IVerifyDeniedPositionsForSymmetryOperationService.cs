using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public interface IVerifyDeniedPositionsForSymmetryOperationService : IOperation<VerifyDeniedPositionsForSymmetryIdentity>
    {
        void VerifyWithinCollection(IEnumerable<DeniedPosition> deniedPositions);
    }
}
