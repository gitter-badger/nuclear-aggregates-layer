using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages
{
    public sealed class OperationProgressMessage : MessageBase<FreeProcessingModel>
    {
        private readonly Guid _operationToken;
        private readonly IOperationResult[] _results;

        public OperationProgressMessage(Guid operationToken, IOperationResult[] results)
            : base(null)
        {
            _operationToken = operationToken;
            _results = results;
        }

        public Guid OperationToken
        {
            get { return _operationToken; }
        }

        public IOperationResult[] Results
        {
            get { return _results; }
        }
    }
}
