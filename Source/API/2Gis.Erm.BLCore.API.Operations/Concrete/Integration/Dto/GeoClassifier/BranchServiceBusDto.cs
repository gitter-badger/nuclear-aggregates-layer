using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Flows;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.GeoClassifier
{
    [ServiceBusObjectDescription("Branch")]
    public class BranchServiceBusDto : IServiceBusDto<FlowGeoClassifier>
    {
        public int Code { get; set; }
        public string DisplayName { get; set; }
        public string NameLat { get; set; }
        public bool IsDeleted { get; set; }
        public string DefaultLang { get; set; }
    }
}