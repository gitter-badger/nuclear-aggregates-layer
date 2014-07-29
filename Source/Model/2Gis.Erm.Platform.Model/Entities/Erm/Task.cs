using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Task : ActivityBase
    {
	    public Task() : base(ActivityType.Task)
	    {
	    }

	    public ActivityTaskType TaskType { get; set; }
    }
}
