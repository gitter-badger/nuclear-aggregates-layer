using DoubleGis.Erm.BLCore.API.Operations.DI;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BL.Operations.DI
{
    public class BlOperationsAssembly : IZoneAssembly<OperationsZone>,
                                        IZoneAnchor<OperationsZone>,
                                        IContainsType<IRequestHandler>
    {
    }
}