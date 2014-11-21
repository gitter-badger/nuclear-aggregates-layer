using System;
using System.IO;
using System.Linq.Expressions;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus
{
    public sealed class WriteMessageToServiceBusRequest : ExportRequest
    {
        public Stream MessageStream { get; set; }
        public string FlowName { get; set; }
        public Expression<Func<string>> XsdSchemaResourceExpression { get; set; } 
    }
}