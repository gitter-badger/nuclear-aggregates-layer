using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IActivityStateAspect : IAspect
    {
        ActivityStatus Status { get; }
    }
}
