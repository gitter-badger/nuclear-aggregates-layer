using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Shared;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.AdvModelsInfo
{
    [ServiceBusObjectDescription("AdvModelInRubricInfo")]
    public sealed class AdvModelInRubricInfoServiceBusDto : IServiceBusDto<FlowAdvModelsInfo>
    {
        public long BranchCode { get; set; }
        public IEnumerable<AdvModelInRubricDto> AdvModelInRubrics { get; set; }
    }

    public sealed class AdvModelInRubricDto 
    {
        public long RubricCode { get; set; }
        public ServiceBusSalesModel AdvModel { get; set; }
    }
}