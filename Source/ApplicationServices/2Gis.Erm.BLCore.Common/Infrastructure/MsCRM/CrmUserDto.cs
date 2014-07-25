using System;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto
{
    public class CrmUserDto
    {
        public Guid CrmUserId { get; set; }
        public TimeSpan TimeZoneTotalBias { get; set; }
    }
}
