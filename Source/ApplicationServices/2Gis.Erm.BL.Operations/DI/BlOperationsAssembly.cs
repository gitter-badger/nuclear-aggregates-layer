using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.DI;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BL.Operations.DI
{
    public sealed class BlOperationsAssembly : IZoneAssembly<OperationsZone>,
                                               IZoneAnchor<OperationsZone>,
                                               IContainsType<IOperation>,
                                               IContainsType<IRequestHandler>,
                                               IContainsType<IDeserializeServiceBusObjectService>,
                                               IContainsType<IImportServiceBusDtoService>
    {
    }
}