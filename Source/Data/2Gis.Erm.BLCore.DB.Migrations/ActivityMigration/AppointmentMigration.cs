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
    using CrmAppointmentMetadata = Metadata.Crm.Appointment;
    using CrmAppointmentState = Microsoft.Crm.SdkTypeProxy.AppointmentState;
    using ErmEntityName = Metadata.Erm.EntityName;
    using ErmAppointmentPriority = Metadata.Erm.ActivityPriority;
    using ErmAppointmentStatus = Metadata.Erm.ActivityStatus;
    using ErmAppointmentPurpose = Metadata.Erm.ActivityPurpose;

    [Migration(23483, "Migrates the appointments from CRM to ERM.", "s.pomadin")]
    public sealed class AppointmentMigration : ActivityMigration<AppointmentMigration.Appointment>
    {
        private const string InsertEntityTemplate = @"
INSERT INTO [Activity].[AppointmentBase]
	([Id],[ReplicationCode],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn],[IsActive],[IsDeleted],[OwnerCode],
	 [Subject],[Description],[ScheduledStart],[ScheduledEnd],[Priority],[Status],[Location],[Purpose])
	VALUES ({0}, {1}, {2}, {3}, {4}, {5}, 1, 0, {6},
			{7}, {8}, {9}, {10}, {11}, {12}, {13}, {14})";
        private const string InsertReferenceTemplate = @"
INSERT INTO [Activity].[AppointmentReferences]
    ([AppointmentId],[Reference],[ReferencedType],[ReferencedObjectId])
	VALUES ({0}, {1}, {2}, {3})";

        internal override QueryExpression CreateQuery()
        {
            var query = new QueryExpression
            {
                EntityName = CrmEntityName.appointment.ToString(),
                ColumnSet = new ColumnSet(new[]
						{
							CrmAppointmentMetadata.ActivityId,
							CrmAppointmentMetadata.CreatedBy,
							CrmAppointmentMetadata.CreatedOn,
							CrmAppointmentMetadata.ModifiedBy,
							CrmAppointmentMetadata.ModifiedOn,
							CrmAppointmentMetadata.OwnerId,
							CrmAppointmentMetadata.RegardingObjectId,
							CrmAppointmentMetadata.Subject,
							CrmAppointmentMetadata.Description,
							CrmAppointmentMetadata.ScheduledStart,
							CrmAppointmentMetadata.ScheduledEnd,
							CrmAppointmentMetadata.PriorityCode,
							CrmAppointmentMetadata.StateCode,
							CrmAppointmentMetadata.Location,
							CrmAppointmentMetadata.Purpose,
							CrmAppointmentMetadata.Organizer,
							CrmAppointmentMetadata.RequiredAttendees,
							CrmAppointmentMetadata.OptionalAttendees,
						}),
            };
            return query;
        }

        internal override Appointment ParseActivity(IActivityMigrationContextExtended context, DynamicEntity entity)
        {
            return Appointment.Create(context, entity);
        }

        internal override string BuildSql(Appointment appointment)
        {
            var sb = new StringBuilder();

            sb.AppendLine(QueryBuilder.Format(InsertEntityTemplate,
                appointment.Id, appointment.ReplicationCode,
                appointment.CreatedBy, appointment.CreatedOn, appointment.ModifiedBy, appointment.ModifiedOn, appointment.OwnerId,
                appointment.Subject, appointment.Description, appointment.ScheduledStart, appointment.ScheduledEnd, appointment.Priority, appointment.Status,
                appointment.Location, appointment.Purpose));

            // regarding object
            foreach (var regardingObject in appointment.RegardingObjects)
            {
                BuildSqlStatement<long>(InsertReferenceTemplate, appointment.Id, AppointmentReferenceType.RegardingObject, regardingObject.EntityName, regardingObject.EntityId).DoIfNotNull(x => sb.AppendLine(x));
            }
            // organizer
            BuildSqlStatement(InsertReferenceTemplate, appointment.Id, AppointmentReferenceType.Organizer, ErmEntityName.User, appointment.OrganizerId).DoIfNotNull(x => sb.AppendLine(x));
            // attendees
            foreach (var attendee in appointment.RequiredAttendees)
            {
                BuildSqlStatement<long>(InsertReferenceTemplate, appointment.Id, AppointmentReferenceType.RequiredAttendees, attendee.EntityName, attendee.EntityId).DoIfNotNull(x => sb.AppendLine(x));
            }

            return sb.ToString();
        }

        #region Appointment

        [DebuggerDisplay("Appointment: {Subject}, {Status}")]
        public sealed class Appointment
        {
            private Appointment()
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
            public long? OrganizerId { get; private set; }
            public IEnumerable<ActivityReference> RequiredAttendees { get; private set; }

            public string Subject { get; private set; }
            public string Description { get; private set; }
            public DateTime? ScheduledStart { get; private set; }
            public DateTime? ScheduledEnd { get; private set; }
            public ErmAppointmentPriority Priority { get; private set; }
            public ErmAppointmentStatus Status { get; private set; }
            public string Location { get; private set; }
            public ErmAppointmentPurpose Purpose { get; private set; }

            internal static Appointment Create(IActivityMigrationContextExtended context, DynamicEntity entity)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                if (entity == null)
                    throw new ArgumentNullException("entity");
                if (entity.Name != CrmEntityName.appointment.ToString())
                    throw new ArgumentException("The specified entity is not an appointment.", "entity");

                var regardingObjects = new[] { entity.Value(CrmAppointmentMetadata.RegardingObjectId) as CrmReference };
                var attendees = (entity.Value(CrmAppointmentMetadata.RequiredAttendees) as DynamicEntity[]).EnumerateActivityReferences()
                        .Concat((entity.Value(CrmAppointmentMetadata.OptionalAttendees) as DynamicEntity[]).EnumerateActivityReferences()).ToList();

                var appointment = new Appointment
                {
                    Id = context.NewIdentity(),
                    ReplicationCode = context.Parse<Guid>(entity.Value(CrmAppointmentMetadata.ActivityId)),
                    CreatedBy = context.Parse<long>(entity.Value(CrmAppointmentMetadata.CreatedBy)),
                    CreatedOn = context.Parse<DateTime>(entity.Value(CrmAppointmentMetadata.CreatedOn)),
                    ModifiedBy = context.Parse<long>(entity.Value(CrmAppointmentMetadata.ModifiedBy)),
                    ModifiedOn = context.Parse<DateTime>(entity.Value(CrmAppointmentMetadata.ModifiedOn)),
                    OwnerId = context.Parse<long?>(entity.Value(CrmAppointmentMetadata.OwnerId)),
                    Subject = context.Parse<string>(entity.Value(CrmAppointmentMetadata.Subject)),
                    Description = context.Parse<string>(entity.Value(CrmAppointmentMetadata.Description)),
                    ScheduledStart = context.Parse<DateTime>(entity.Value(CrmAppointmentMetadata.ScheduledStart)),
                    ScheduledEnd = context.Parse<DateTime>(entity.Value(CrmAppointmentMetadata.ScheduledEnd)),
                    Priority = context.Parse<int>(entity.Value(CrmAppointmentMetadata.PriorityCode)).Map(ToPriority),
                    Status = context.Parse<CrmAppointmentState>(entity.Value(CrmAppointmentMetadata.StateCode)).Map(ToStatus),
                    Location = context.Parse<string>(entity.Value(CrmAppointmentMetadata.Location)),
                    Purpose = context.Parse<int>(entity.Value(CrmAppointmentMetadata.Purpose)).Map(ToPurpose),

                    // requirement: привязанным объектом м.б. только клиент, фирма или сделка
                    RegardingObjects = regardingObjects.Concat(attendees)
                        .FilterByEntityName(ErmEntityName.Client, ErmEntityName.Firm, ErmEntityName.Deal)
                        .Select(x => x.ToReferenceWithin(context))
                        .Distinct() // it's safe as ActivityReference implements IEquatable<>
                        .ToList(),
                    // requirement: организатором могут быть только пользователи
                    OrganizerId = (entity.Value(CrmAppointmentMetadata.Organizer) as DynamicEntity[]).EnumerateActivityReferences()
                        .FilterByEntityName(ErmEntityName.User)
                        .Select(context.Parse<long?>)
                        .FirstOrDefault(),
                    // requirement: участниками могут быть только контакты
                    RequiredAttendees = attendees.Concat(regardingObjects)
                        .FilterByEntityName(ErmEntityName.Contact)
                        .Select(x => x.ToReferenceWithin(context))
                        .Distinct() // it's safe as ActivityReference implements IEquatable<>
                        .ToList(),
                };

                return appointment;
            }

            private static ErmAppointmentPriority ToPriority(int code)
            {
                switch (code)
                {
                    case 0:
                        return ErmAppointmentPriority.Low;
                    case 1:
                        return ErmAppointmentPriority.Average;
                    case 2:
                        return ErmAppointmentPriority.High;
                    default:
                        return ErmAppointmentPriority.NotSet;
                }
            }

            private static ErmAppointmentStatus ToStatus(CrmAppointmentState state)
            {
                switch (state)
                {
                    case CrmAppointmentState.Open:
                    case CrmAppointmentState.Scheduled:
                        return ErmAppointmentStatus.InProgress;
                    case CrmAppointmentState.Completed:
                        return ErmAppointmentStatus.Completed;
                    case CrmAppointmentState.Canceled:
                        return ErmAppointmentStatus.Canceled;
                    default:
                        return ErmAppointmentStatus.NotSet;
                }
            }

            private static ErmAppointmentPurpose ToPurpose(int code)
            {
                switch (code)
                {
                    case 3:
                        return ErmAppointmentPurpose.ProductPresentation;
                    case 4:
                        return ErmAppointmentPurpose.OpportunitiesPresentation;
                    case 5:
                        return ErmAppointmentPurpose.OfferApproval;
                    case 6:
                        return ErmAppointmentPurpose.DecisionApproval;
                    case 7:
                        return ErmAppointmentPurpose.Prolongation;
                    case 8:
                        return ErmAppointmentPurpose.Service;
                    case 9:
                        return ErmAppointmentPurpose.Upsale;
                    default:
                        return ErmAppointmentPurpose.NotSet;
                }
            }
        }

        internal enum AppointmentReferenceType
        {
            RegardingObject = 1,
            Organizer = 2,
            RequiredAttendees = 3,
        }

        #endregion
    }
}