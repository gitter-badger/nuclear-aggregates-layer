using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO.FirmInfo
{
    public class FirmWithAddressesAndProjectDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<FirmAddressWithCategoriesDto> FirmAddresses { get; set; }
        public ProjectDto Project { get; set; }
        public string Owner { get; set; }
        public long OwnerCode { get; set; }
    }
}