using System;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto
{
    // TODO {d.ivanov, 25.11.2013}: должен лечь в 2Gis.Erm.BLCore.API.Operations\Integration\Replication\Dto\HotClientRequestDto.cs
    public sealed class HotClientRequestDto
    {
        public long Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string ContactPhone { get; set; }
        public string ContactName { get; set; }
        public string Description { get; set; }
    }
}
