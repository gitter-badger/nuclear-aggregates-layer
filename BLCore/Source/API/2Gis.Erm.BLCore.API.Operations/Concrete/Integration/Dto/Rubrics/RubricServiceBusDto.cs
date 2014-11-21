using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Rubrics
{
    [ServiceBusObjectDescription("Rubric")]
    public sealed class RubricServiceBusDto : IServiceBusDto<FlowRubrics>
    {
        public string Comment { get; set; }
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHidden { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public IEnumerable<int> OrganizationUnitsDgppIds { get; set; }
        public long? ParentId { get; set; }
    }
}