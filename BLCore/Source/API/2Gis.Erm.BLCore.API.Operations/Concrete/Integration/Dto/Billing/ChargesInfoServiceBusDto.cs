using System;
using System.Collections.Generic;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Billing
{
    [ServiceBusObjectDescription("ChargesInfo")]
    public sealed class ChargesInfoServiceBusDto : IServiceBusDto<FlowBilling>
    {
        public long BranchCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IReadOnlyCollection<ChargeDto> Charges { get; set; }
        public XElement Content { get; set; }
        public Guid SessionId { get; set; }
    }
}