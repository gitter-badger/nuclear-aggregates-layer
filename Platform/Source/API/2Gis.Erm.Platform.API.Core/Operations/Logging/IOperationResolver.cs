using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public interface IOperationResolver
    {
        StrictOperationIdentity ResolveOperation(PerformedBusinessOperation operation);
    }
}