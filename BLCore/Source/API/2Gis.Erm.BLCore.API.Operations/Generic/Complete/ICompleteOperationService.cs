using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Complete
{
    public interface ICompleteOperationService : IOperation<CompleteIdentity>
    {
        void Complete(long entityId);
    }
}
