using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Metadata.Security;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Core.Metadata.Security
{
    public interface IOperationAccessRequirement
    {
        StrictOperationIdentity StrictOperationIdentity { get; }
        IEnumerable<StrictOperationIdentity> UsedOperations { get; }
        IEnumerable<IAccessRequirement> Requirements { get; }
    }
}
