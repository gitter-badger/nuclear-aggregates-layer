using System;
using System.ServiceModel;

namespace DoubleGis.Erm.Platform.API.Core.Operations
{
    public interface IOperationProgressCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyAboutProgress(Guid operationToken, IOperationResult[] results);
    }
}