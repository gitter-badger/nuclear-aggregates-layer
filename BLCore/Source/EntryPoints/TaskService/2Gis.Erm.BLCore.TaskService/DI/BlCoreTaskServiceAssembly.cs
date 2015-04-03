using NuClear.Assembling.Zones;
using DoubleGis.Erm.Platform.TaskService.Jobs;

namespace DoubleGis.Erm.BLCore.TaskService.DI
{
    public sealed class BlCoreTaskServiceAssembly : IZoneAssembly<TaskServiceZone>,
                                                    IZoneAnchor<TaskServiceZone>,
                                                    IContainsType<ITaskServiceJob>
    {
    }
}