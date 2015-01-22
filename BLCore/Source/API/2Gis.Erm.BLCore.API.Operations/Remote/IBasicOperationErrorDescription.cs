using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote
{
    public interface IBasicOperationErrorDescription
    {
        IEntityType EntityName { get; }
        string Message { get; }
    }
}