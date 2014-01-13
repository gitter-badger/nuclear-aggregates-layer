using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.DTO
{
    public class ActivityInstanceDto
    {
        public ActivityInstance ActivityInstance { get; set; }
        public ICollection<ActivityPropertyInstance> ActivityPropretyInstances { get; set; }
    }
}