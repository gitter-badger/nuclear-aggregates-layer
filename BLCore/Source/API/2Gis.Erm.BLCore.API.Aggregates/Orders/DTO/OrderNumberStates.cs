using System;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO
{
    [Flags]
    public enum OrderNumberStates
    {
        // Основные
        HasSuffix = 1,
        HasPlatform = 2,

        // Производные
        NoSuffixNoPlatform = 0,
        HasSuffixNoPlatform = HasSuffix,
        HasSuffixHasPlatform = HasSuffix | HasPlatform,
        NoSuffixHasPlatform = HasPlatform,
    }
}