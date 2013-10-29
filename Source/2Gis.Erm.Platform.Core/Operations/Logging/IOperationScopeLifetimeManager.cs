using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public interface IOperationScopeLifetimeManager
    {
        void Close(IOperationScope scope);
    }
}