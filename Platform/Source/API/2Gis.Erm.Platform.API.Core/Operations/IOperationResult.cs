namespace DoubleGis.Erm.Platform.API.Core.Operations
{
    public interface IOperationResult
    {
        long EntityId { get; }
        bool Succeeded { get; }
        string Message { get; }
    }
}