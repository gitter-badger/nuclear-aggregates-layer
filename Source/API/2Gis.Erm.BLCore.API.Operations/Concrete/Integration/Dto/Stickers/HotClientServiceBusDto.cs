using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Stickers
{
    [ServiceBusObjectDescription("HotClient")]
    public class HotClientServiceBusDto : IServiceBusDto<FlowStickers>
    {
        public string SourceCode { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public DateTime CreationDate { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string Description { get; set; }
        public long? CardCode { get; set; }
        public long? BranchCode { get; set; }
    }
}