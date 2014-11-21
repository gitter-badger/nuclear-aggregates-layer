using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Metadata.Security
{
    public interface IOperationSecurityRegistryReader
    {
        bool TryGetOperationRequirements(StrictOperationIdentity operationIdentity, out IEnumerable<IAccessRequirement> securityRequirements);
    }
}
