using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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
                ? new Task
                    {
                        CreatedBy = dto.CreatedByRef.GetId(),
                        CreatedOn = dto.CreatedOn,
                        ModifiedBy = dto.ModifiedByRef.GetId(),
                        ModifiedOn = dto.ModifiedOn,
                        Timestamp = dto.Timestamp,
                        // TODO {s.pomadin, 05.09.2014}: could IsActive or IsDeleted be set via UI directly?
                        IsActive = dto.IsActive,
                        IsDeleted = dto.IsDeleted,
                    }
                : _finder.FindOne(Specs.Find.ById<Task>(dto.Id));

            task.Header = dto.Header;
            task.TaskType = dto.TaskType;
            task.Priority = dto.Priority;
            task.Description = dto.Description;
            task.ScheduledOn = dto.ScheduledOn;
            task.Status = dto.Status;
            task.OwnerCode = dto.OwnerRef.GetId();

            return task;
        }
    }
}
