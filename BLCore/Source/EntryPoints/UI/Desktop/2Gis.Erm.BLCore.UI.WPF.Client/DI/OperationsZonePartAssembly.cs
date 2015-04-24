using DoubleGis.Erm.BLCore.API.Operations.DI;
using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI
{
    public sealed class OperationsZonePartAssembly : IZoneAssembly<OperationsZone>,
                                                     IZoneAnchor<OperationsZone>,
                                                     IContainsType<IOperation>
    {
    }
}
