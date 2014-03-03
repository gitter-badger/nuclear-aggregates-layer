using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.DTO.FirmInfo
{
    public class FirmWithAddressesAndProjectDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<FirmAddressWithCategoriesDto> FirmAddresses { get; set; }
        public ProjectDto Project { get; set; }
    }
}