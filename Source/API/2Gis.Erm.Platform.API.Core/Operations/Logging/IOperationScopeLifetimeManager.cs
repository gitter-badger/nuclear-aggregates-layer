namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public interface IOperationScopeLifetimeManager
    {
        void Close(IOperationScope scope);
    }
}