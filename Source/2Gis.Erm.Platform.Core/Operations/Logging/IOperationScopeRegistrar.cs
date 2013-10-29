using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public interface IOperationScopeRegistrar
    {
        void RegisterRoot(IOperationScope rootScope);
        void RegisterChild(IOperationScope childScope, IOperationScope parentScope);
        void RegisterAutoResolvedChild(IOperationScope childScope);
        void Unregister(IOperationScope scope);
    }
}