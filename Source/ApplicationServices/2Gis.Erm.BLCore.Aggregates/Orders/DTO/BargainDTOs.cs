using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.DTO
{
    public sealed class BargainUsageDto
    {
        public Bargain Bargain { get; set; }
        public IEnumerable<string> OrderNumbers { get; set; }
    }
}