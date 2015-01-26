using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface IActivityStateAspect : IAspect
    {
        ActivityStatus Status { get; }
    }
}
