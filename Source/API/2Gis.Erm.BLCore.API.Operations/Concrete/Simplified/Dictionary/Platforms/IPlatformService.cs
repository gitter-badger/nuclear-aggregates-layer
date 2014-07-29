using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Platforms.DTO;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Platforms
{
    public interface IPlatformService : ISimplifiedModelConsumer
    {
        int Delete(Erm.Platform.Model.Entities.Erm.Platform platform);
        PlatwormWithPositionsDto GetPlatformWithPositions(long entityId);
        Erm.Platform.Model.Entities.Erm.Platform GetPlatform(long entityId);
        void CreateOrUpdate(Erm.Platform.Model.Entities.Erm.Platform platform);
        bool IsPlatformLinked(long platformId);
    }
}
