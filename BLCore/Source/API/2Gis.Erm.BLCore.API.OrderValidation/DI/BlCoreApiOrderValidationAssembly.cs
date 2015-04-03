using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.DI
{
    public sealed class BlCoreApiOrderValidationAssembly : IZoneAssembly<OrderValidationZone>,
                                                           IZoneAnchor<OrderValidationZone>,
                                                           IContainsType<IOperation>
    {
    }
}