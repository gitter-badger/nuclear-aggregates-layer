namespace DoubleGis.Erm.BLCore.API.Operations.Remote
{
    public interface IBasicOperationErrorDescription
    {
        string EntityName { get; }
        string Message { get; }
    }
}