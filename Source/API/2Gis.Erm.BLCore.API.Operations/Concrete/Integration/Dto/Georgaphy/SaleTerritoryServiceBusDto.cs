using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Georgaphy
{
    [ServiceBusObjectDescription("SaleTerritory")]
    public sealed class SaleTerritoryServiceBusDto : IServiceBusDto<FlowGeography>
    {
        public long Code { get; set; }
        public string Name { get; set; }
        public int OrganizationUnitDgppId { get; set; }
        public bool IsDeleted { get; set; }
    }
}