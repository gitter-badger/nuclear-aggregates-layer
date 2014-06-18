using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared
{
    public class ImportFirmAddressDto
    {
        public FirmAddress FirmAddress { get; set; }
        public int BranchCode { get; set; }
    }
}