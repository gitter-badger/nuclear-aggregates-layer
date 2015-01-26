using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface ILimitStateAspect : IAspect
    {
        LimitStatus Status { get; }
    }
}