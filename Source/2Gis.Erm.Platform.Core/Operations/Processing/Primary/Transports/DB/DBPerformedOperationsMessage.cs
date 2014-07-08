using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.DB
{
    public sealed class DBPerformedOperationsMessage : MessageBase
    {
        private readonly Guid _id;
        private readonly IEnumerable<PerformedBusinessOperation> _operations;

        public DBPerformedOperationsMessage(IEnumerable<PerformedBusinessOperation> operations)
        {
            _id = operations.First().UseCaseId;
            _operations = operations;
        }

        public override Guid Id
        {
            get { return _id; }
        }

        public IEnumerable<PerformedBusinessOperation> Operations
        {
            get { return _operations; }
        }
    }
}
