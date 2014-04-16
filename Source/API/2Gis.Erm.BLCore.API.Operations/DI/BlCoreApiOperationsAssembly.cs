using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BLCore.API.Operations.DI
{
    public sealed class BlCoreApiOperationsAssembly : IZoneAssembly<OperationsZone>,
                                                      IZoneAnchor<OperationsZone>,
                                                      IContainsType<IOperation>
    {
    }
}