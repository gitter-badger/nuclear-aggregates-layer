using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Extensions;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration
{
    using CrmEntityName = Microsoft.Crm.SdkTypeProxy.EntityName;
    using CrmTaskMetadata = Metadata.Crm.Task;
    using CrmTaskState = Microsoft.Crm.SdkTypeProxy.TaskState;
    using ErmEntityName = Metadata.Erm.EntityName;
    using ErmTaskPriority = Metadata.Erm.ActivityPriority;
    using ErmTaskStatus = Metadata.Erm.ActivityStatus;
    using ErmTaskType = Metadata.Erm.TaskType;

    [Migration(23486, "Migrates the tasks from CRM to ERM.", "s.pomadin")]
    public sealed class TaskMigration : ActivityMigration<TaskMigration.Task>
    {
        private const string InsertEntityTemplate = @"
INSERT INTO [Activity].[TaskBase]
	([Id],[ReplicationCode],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn],[IsActive],[IsDeleted],[OwnerCode],
	 [Subject],[Description],[ScheduledOn],[Priority],[Status],[TaskType])
	VALUES ({0}, {1}, {2}, {3}, {4}, {5}, 1, 0, {6},
			{7}, {8}, {9}, {10}, {11}, {12})";
        private const string InsertReferenceTemplate = @"
INSERT INTO [Activity].[TaskReferences]
           ([TaskId],[Reference],[ReferencedType],[ReferencedObjectId])
Values ({0}, {1}, {2}, {3})";

        internal override QueryExpression CreateQuery()
        {
            var query = new QueryExpression
                            {
                                EntityName = CrmEntityName.task.ToString(),
                                ColumnSet = new ColumnSet(new[]
                                                              {
                                                                  CrmTaskMetadata.ActivityId,
                                                                  CrmTaskMetadata.CreatedBy,
                                                                  CrmTaskMetadata.CreatedOn,
                                                                  CrmTaskMetadata.ModifiedBy,
                                                                  CrmTaskMetadata.ModifiedOn,
                                                                  CrmTaskMetadata.OwnerId,
                                                                  CrmTaskMetadata.RegardingObjectId,
                                                                  CrmTaskMetadata.Subject,
                                                                  CrmTaskMetadata.Description,
                                                                  CrmTaskMetadata.ScheduledStart,
                                                                  CrmTaskMetadata.PriorityCode,
                                                                  CrmTaskMetadata.StateCode,
                                                                  CrmTaskMetadata.TaskType,
                                                              }),
                            };
            return query;
        }

        internal override Task ParseActivity(IActivityMigrationContextExtended context, DynamicEntity entity)
        {
            return Task.Create(context, entity);
        }

        internal override string BuildSql(Task task)
        {
            var sb = new StringBuilder();

            sb.AppendLine(QueryBuilder.Format(InsertEntityTemplate,
                                              task.Id,
                                              task.ReplicationCode,
                                              task.CreatedBy,
                                              task.CreatedOn,
                                              task.ModifiedBy,
                                              task.ModifiedOn,
                                              task.OwnerId,
                                              task.Subject,
                                              task.Description,
                                              task.ScheduledOn,
                                              task.Priority,
                                              task.Status,
                                              task.TaskType));

            // regarding object
            foreach (var regardingObject in task.RegardingObjects)
            {
                BuildSqlStatement<long>(InsertReferenceTemplate, task.Id, TaskReferenceType.RegardingObject, regardingObject.EntityName, regardingObject.EntityId).DoIfNotNull(x => sb.AppendLine(x));
            }

            return sb.ToString();
        }

        #region Task

        [DebuggerDisplay("Task: {Subject}, {Status}")]
        public sealed class Task
        {
            private Task()
            {
            }

            public long Id { get; private set; }
            public long CreatedBy { get; private set; }
            public DateTime CreatedOn { get; private set; }
            public long ModifiedBy { get; private set; }
            public DateTime ModifiedOn { get; private set; }
            public Guid ReplicationCode { get; private set; }

            public long? OwnerId { get; private set; }
            public IEnumerable<ActivityReference> RegardingObjects { get; private set; }

            public string Subject { get; private set; }
            public string Description { get; private set; }
            public DateTime? ScheduledOn { get; private set; }
            public ErmTaskPriority Priority { get; private set; }
            public ErmTaskStatus Status { get; private set; }
            public ErmTaskType TaskType { get; private set; }

            internal static Task Create(IActivityMigrationContextExtended context, DynamicEntity entity)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                if (entity.Name != CrmEntityName.task.ToString())
                {
                    throw new ArgumentException("The specified entity is not a task.", "entity");
                }

                var task = new Task
                {
                    Id = context.NewIdentity(),
                    ReplicationCode = context.Parse<Guid>(entity.Value(CrmTaskMetadata.ActivityId)),
                    CreatedBy = context.Parse<long>(entity.Value(CrmTaskMetadata.CreatedBy)),
                    CreatedOn = context.Parse<DateTime>(entity.Value(CrmTaskMetadata.CreatedOn)),
                    ModifiedBy = context.Parse<long>(entity.Value(CrmTaskMetadata.ModifiedBy)),
                    ModifiedOn = context.Parse<DateTime>(entity.Value(CrmTaskMetadata.ModifiedOn)),
                    OwnerId = context.Parse<long?>(entity.Value(CrmTaskMetadata.OwnerId)),
                    Subject = context.Parse<string>(entity.Value(CrmTaskMetadata.Subject)),
                    Description = context.Parse<string>(entity.Value(CrmTaskMetadata.Description)),
                    Priority = context.Parse<int>(entity.Value(CrmTaskMetadata.PriorityCode)).Map(ToPriority),
                    Status = context.Parse<CrmTaskState>(entity.Value(CrmTaskMetadata.StateCode)).Map(ToStatus),
                    TaskType = context.Parse<int>(entity.Value(CrmTaskMetadata.TaskType)).Map(ToType),
                    
                    // it might have empty schedule time
                    ScheduledOn = context.Parse<DateTime?>(entity.Value(CrmTaskMetadata.ScheduledStart))
                        ?? context.Parse<DateTime?>(entity.Value(CrmTaskMetadata.ActualStart))
                        ?? context.Parse<DateTime?>(entity.Value(CrmTaskMetadata.ActualEnd))
                        ?? context.Parse<DateTime>(entity.Value(CrmTaskMetadata.ModifiedOn)),

                    // requirement: привязанным объектом м.б. только клиент, фирма или сделка
                    RegardingObjects = new[] { entity.Value(CrmTaskMetadata.RegardingObjectId) as CrmReference }
                        .FilterByEntityName(ErmEntityName.Client, ErmEntityName.Firm, ErmEntityName.Deal)
                        .Select(x => x.ToReferenceWithin(context))
                        .ToList(),
                };

                return task;
            }

            private static ErmTaskPriority ToPriority(int code)
            {
                switch (code)
                {
                    case 0:
                        return ErmTaskPriority.Low;
                    case 1:
                        return ErmTaskPriority.Average;
                    case 2:
                        return ErmTaskPriority.High;
                    default:
                        return ErmTaskPriority.NotSet;
                }
            }

            private static ErmTaskStatus ToStatus(CrmTaskState state)
            {
                switch (state)
                {
                    case CrmTaskState.Open:
                        return ErmTaskStatus.InProgress;
                    case CrmTaskState.Completed:
                        return ErmTaskStatus.Completed;
                    case CrmTaskState.Canceled:
                        return ErmTaskStatus.Canceled;
                    default:
                        return ErmTaskStatus.NotSet;
                }
            }

            private static ErmTaskType ToType(int code)
            {
                switch (code)
                {
                    case 1:
                    case 4:
                        return ErmTaskType.WarmClient;
                    case 2:
                        return ErmTaskType.BossTask;
                    case 3:
                        return ErmTaskType.Other;
                    default:
                        return ErmTaskType.NotSet;
                }
            }
        }

        internal enum TaskReferenceType
        {
            RegardingObject = 1,
        }

        #endregion
    }
}