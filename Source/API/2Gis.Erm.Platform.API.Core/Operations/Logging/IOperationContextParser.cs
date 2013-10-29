using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public interface IOperationContextParser
    {
        IReadOnlyDictionary<StrictOperationIdentity, IEnumerable<long>> GetGroupedIdsFromContext(string context, int operation, int descriptor);
    }
}