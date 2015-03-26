using DoubleGis.Erm.BLCore.TaskService.DI;
using DoubleGis.Erm.Platform.Model.Zones;

using NuClear.Jobs;

namespace DoubleGis.Erm.BLQuerying.TaskService.DI
{
    public sealed class BlQueryingTaskServiceAssembly : IZoneAssembly<TaskServiceZone>,
                                                        IZoneAnchor<TaskServiceZone>,
                                                        IContainsType<ITaskServiceJob>
    {
    }
}