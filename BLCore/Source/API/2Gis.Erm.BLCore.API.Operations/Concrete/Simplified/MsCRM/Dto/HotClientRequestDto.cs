using System;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto
{
    public sealed class HotClientRequestDto
    {
        public long Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string ContactPhone { get; set; }
        public string ContactName { get; set; }
        public string Description { get; set; }
    }
}
