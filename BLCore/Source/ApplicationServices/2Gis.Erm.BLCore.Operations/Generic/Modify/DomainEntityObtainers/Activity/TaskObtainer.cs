using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class TaskObtainer : IBusinessModelEntityObtainer<Task>, IAggregateReadModel<Task>
    {
        private readonly IFinder _finder;

        public TaskObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Task ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (TaskDomainEntityDto)domainEntityDto;

            var task = dto.IsNew()
                ? new Task { IsActive = true, Status = dto.Status, OwnerCode = dto.OwnerRef.GetId() } 
                : _finder.FindOne(Specs.Find.ById<Task>(dto.Id));

            task.Header = dto.Header;
            task.TaskType = dto.TaskType;
            task.Priority = dto.Priority;
            task.Description = dto.Description;
            task.ScheduledOn = dto.ScheduledOn;
            task.Timestamp = dto.Timestamp;

            return task;
        }
    }
}
