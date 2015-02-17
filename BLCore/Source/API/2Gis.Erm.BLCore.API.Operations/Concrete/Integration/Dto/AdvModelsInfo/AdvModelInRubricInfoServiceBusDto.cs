using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.AdvModelsInfo
{
    public enum AdvModel
    {
        Cps = 10,
        Fh = 11,
        Mfh = 12
    }

    [ServiceBusObjectDescription("AdvModelInRubricInfo")]
    public sealed class AdvModelInRubricInfoServiceBusDto : IServiceBusDto<FlowAdvModelsInfo>
    {
        public long BranchCode { get; set; }
        public IEnumerable<AdvModelInRubricDto> AdvModelInRubrics { get; set; }
    }

    public sealed class AdvModelInRubricDto 
    {
        public long RubricCode { get; set; }
        public AdvModel AdvModel { get; set; }
    }
}