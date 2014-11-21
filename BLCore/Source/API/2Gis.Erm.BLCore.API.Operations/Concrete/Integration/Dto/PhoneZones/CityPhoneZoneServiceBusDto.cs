using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.PhoneZones
{
    [ServiceBusObjectDescription("CityPhoneZone")]
    public class CityPhoneZoneServiceBusDto : IServiceBusDto<FlowPhoneZones>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CityCode { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsDeleted { get; set; }
    }
}