using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.Dgpp
{
    public sealed class ExportFirmsWithActiveOrdersRequest : Request 
    {
        public int OrganizationUnitId { get; set; }
    }
}