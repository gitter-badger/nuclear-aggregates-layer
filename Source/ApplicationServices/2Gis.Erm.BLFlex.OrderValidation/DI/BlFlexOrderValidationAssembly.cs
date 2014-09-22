using DoubleGis.Erm.BLCore.API.OrderValidation.DI;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BLFlex.OrderValidation.DI
{
    public sealed class BlFlexOrderValidationAssembly : IZoneAssembly<OrderValidationZone>,
                                                        IZoneAnchor<OrderValidationZone>,
                                                        IContainsType<IMetadataSource>
    {
    }
}