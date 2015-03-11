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
        void CreateOperation(Operation operation, string operationLog, string logfileName, string contentType = MediaTypeNames.Text.Plain);
        void CreateOperation(Operation operation, byte[] logContent, string logfileName, string contentType);
        void CreateOperation(Operation operation, Stream newData, string fileName, string contentType);
    }
}
