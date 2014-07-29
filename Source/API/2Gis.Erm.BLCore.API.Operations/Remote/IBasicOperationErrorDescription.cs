using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote
{
    public interface IBasicOperationErrorDescription
    {
        EntityName EntityName { get; }
        string Message { get; }
    }
}