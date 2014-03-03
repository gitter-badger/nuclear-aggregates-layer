using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.DTO.FirmInfo
{
    public class FirmAddressWithCategoriesDto
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public IEnumerable<CategoryDto> Categories { get; set; }
    }
}