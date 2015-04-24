using DoubleGis.Erm.BLCore.TaskService.DI;

using NuClear.Assembling.Zones;
using NuClear.Jobs;

namespace DoubleGis.Erm.BLQuerying.TaskService.DI
{
    public sealed class BlQueryingTaskServiceAssembly : IZoneAssembly<TaskServiceZone>,
                                                        IZoneAnchor<TaskServiceZone>,
                                                        IContainsType<ITaskServiceJob>
    {
    }
}