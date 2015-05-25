using System.Collections.Generic;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Metadata.Security
{
    public interface IOperationSecurityRegistryReader
    {
        bool TryGetOperationRequirements(StrictOperationIdentity operationIdentity, out IEnumerable<IAccessRequirement> securityRequirements);
    }
}
