using System;

namespace DoubleGis.Erm.Platform.API.Core.Operations
{
    public interface ICommonOperationParameter : IOperationParameter
    {
        Guid OperationToken { get; }
    }
}