using DoubleGis.Erm.Platform.Model.Zones;

using NuClear.Jobs;

namespace DoubleGis.Erm.BLCore.TaskService.DI
{
    public sealed class BlCoreTaskServiceAssembly : IZoneAssembly<TaskServiceZone>,
                                                    IZoneAnchor<TaskServiceZone>,
                                                    IContainsType<ITaskServiceJob>
    {
    }
}