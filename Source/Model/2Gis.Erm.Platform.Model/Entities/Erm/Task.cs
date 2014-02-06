using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Task : ActivityBase
    {
        public ActivityTaskType TaskType { get; set; }
    }
}
