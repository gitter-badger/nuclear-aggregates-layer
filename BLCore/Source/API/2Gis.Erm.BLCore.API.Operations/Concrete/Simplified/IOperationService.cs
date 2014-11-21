using System;
using System.IO;
using System.Net.Mime;

using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified
{
    public interface IOperationService : ISimplifiedModelConsumer, IInvariantSafeCrosscuttingService
    {
        FileWithContent GetLogForOperation(Guid operationId);
        void FinishOperation(Operation operation, string operationLog, string logfileName, string contentType = MediaTypeNames.Text.Plain);
        void FinishOperation(Operation operation, byte[] logContent, string logfileName, string contentType);
        void FinishOperation(Operation operation, Stream newData, string fileName, string contentType);
        void Add(Operation operation);
        void Update(Operation operation);
    }
}
