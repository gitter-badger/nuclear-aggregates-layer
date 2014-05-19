using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Georgaphy
{
    [ServiceBusObjectDescription("Building")]
    public sealed class BuildingServiceBusDto : IServiceBusDto<FlowGeography>
    {
        public long Code { get; set; }
        public long? SaleTerritoryCode { get; set; }
        public bool IsDeleted { get; set; }
    }
}