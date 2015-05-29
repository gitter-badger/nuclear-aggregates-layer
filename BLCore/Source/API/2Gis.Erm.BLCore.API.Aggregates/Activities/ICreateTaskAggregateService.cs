using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface ICreateTaskAggregateService : IAggregateSpecificService<Task, CreateIdentity>
    {
        void Create(Task task);
    }
}