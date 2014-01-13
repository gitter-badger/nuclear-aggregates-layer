using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting
{
    public interface IConcurrentPeriodCounter
    {
        int Count(IEnumerable<TimePeriod> periods, DateTime fullPeriodStart, DateTime fullPeriodEnd);
    }
}
