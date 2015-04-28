using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.DI;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLCore.OrderValidation.DI
{
    public sealed class BlCoreOrderValidationAssembly : IZoneAssembly<OrderValidationZone>,
                                                        IZoneAnchor<OrderValidationZone>,
                                                        IContainsType<IOrderValidationRule>,
                                                        IContainsType<IRequestHandler>,
                                                        IContainsType<IOperation>,
                                                        IContainsType<IMetadataSource>
    {
    }
}