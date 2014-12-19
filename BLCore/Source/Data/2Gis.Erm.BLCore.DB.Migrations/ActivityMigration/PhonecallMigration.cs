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
    using CrmPhonecallMetadata = Metadata.Crm.Phonecall;
    using CrmPhonecallState = Microsoft.Crm.SdkTypeProxy.PhoneCallState;
    using ErmEntityName = Metadata.Erm.EntityName;
    using ErmPhonecallPriority = Metadata.Erm.ActivityPriority;
    using ErmPhonecallPurpose = Metadata.Erm.ActivityPurpose;
    using ErmPhonecallStatus = Metadata.Erm.ActivityStatus;

    [Migration(23484, "Migrates the phonecalls from CRM to ERM.", "s.pomadin")]
    public sealed class PhonecallMigration : ActivityMigration<PhonecallMigration.Phonecall>
    {
        private const string InsertEntityTemplate = @"
INSERT INTO [Activity].[PhonecallBase]
	([Id],[ReplicationCode],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn],[IsActive],[IsDeleted],[OwnerCode],
	 [Subject],[Description],[ScheduledOn],[Priority],[Status],[Purpose])
	VALUES ({0}, {1}, {2}, {3}, {4}, {5}, 1, 0, {6},
			{7}, {8}, {9}, {10}, {11}, {12})";
        private const string InsertReferenceTemplate = @"
INSERT INTO [Activity].[PhonecallReferences]
    ([PhonecallId],[Reference],[ReferencedType],[ReferencedObjectId])
	VALUES ({0}, {1}, {2}, {3})";

        internal override QueryExpression CreateQuery()
        {
            var query = new QueryExpression
                            {
                                EntityName = CrmEntityName.phonecall.ToString(),
                                ColumnSet = new ColumnSet(new[]
                                                              {
                                                                  CrmPhonecallMetadata.ActivityId,
                                                                  CrmPhonecallMetadata.CreatedBy,
                                                                  CrmPhonecallMetadata.CreatedOn,
                                                                  CrmPhonecallMetadata.ModifiedBy,
                                                                  CrmPhonecallMetadata.ModifiedOn,
                                                                  CrmPhonecallMetadata.OwnerId,
                                                                  CrmPhonecallMetadata.RegardingObjectId,
                                                                  CrmPhonecallMetadata.Subject,
                                                                  CrmPhonecallMetadata.Description,
                                                                  CrmPhonecallMetadata.ScheduledStart,
                                                                  CrmPhonecallMetadata.PriorityCode,
                                                                  CrmPhonecallMetadata.StateCode,
                                                                  CrmPhonecallMetadata.To,
                                                                  CrmPhonecallMetadata.Purpose,
                                                              }),
                            };
            return query;
        }

        internal override Phonecall ParseActivity(IActivityMigrationContextExtended context, DynamicEntity entity)
        {
            return Phonecall.Create(context, entity);
        }

        internal override string BuildSql(Phonecall phonecall)
        {
            var sb = new StringBuilder();

            sb.AppendLine(QueryBuilder.Format(InsertEntityTemplate,
                                              phonecall.Id,
                                              phonecall.ReplicationCode,
                                              phonecall.CreatedBy,
                                              phonecall.CreatedOn,
                                              phonecall.ModifiedBy,
                                              phonecall.ModifiedOn,
                                              phonecall.OwnerId,
                                              phonecall.Subject,
                                              phonecall.Description,
                                              phonecall.ScheduledOn,
                                              phonecall.Priority,
                                              phonecall.Status,
                                              phonecall.Purpose));

            // regarding object
            foreach (var regardingObject in phonecall.RegardingObjects)
            {
                BuildSqlStatement<long>(InsertReferenceTemplate, phonecall.Id, PhonecallReferenceType.RegardingObject, regardingObject.EntityName, regardingObject.EntityId).DoIfNotNull(x => sb.AppendLine(x));
            }
            
            // recipients
            foreach (var attendee in phonecall.Recipients)
            {
                BuildSqlStatement<long>(InsertReferenceTemplate, phonecall.Id, PhonecallReferenceType.Recipient, attendee.EntityName, attendee.EntityId).DoIfNotNull(x => sb.AppendLine(x));
            }

            return sb.ToString();
        }

        #region Phonecall

        [DebuggerDisplay("Phonecall: {Subject}, {Status}")]
        public sealed class Phonecall
        {
            private Phonecall()
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
            public IEnumerable<ActivityReference> Recipients { get; set; }

            public string Subject { get; private set; }
            public string Description { get; private set; }
            public DateTime? ScheduledOn { get; private set; }
            public ErmPhonecallPriority Priority { get; private set; }
            public ErmPhonecallStatus Status { get; private set; }
            public ErmPhonecallPurpose Purpose { get; private set; }

            internal static Phonecall Create(IActivityMigrationContextExtended context, DynamicEntity entity)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }
                
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                if (entity.Name != CrmEntityName.phonecall.ToString())
                {
                    throw new ArgumentException("The specified entity is not a phonecall.", "entity");
                }

                var regardingObjects = new[] { entity.Value(CrmPhonecallMetadata.RegardingObjectId) as CrmReference };
                var recipients = (entity.Value(CrmPhonecallMetadata.To) as DynamicEntity[]).EnumerateActivityReferences().ToList();

                var phonecall = new Phonecall
                {
                    Id = context.NewIdentity(),
                    ReplicationCode = context.Parse<Guid>(entity.Value(CrmPhonecallMetadata.ActivityId)),
                    CreatedBy = context.Parse<long>(entity.Value(CrmPhonecallMetadata.CreatedBy)),
                    CreatedOn = context.Parse<DateTime>(entity.Value(CrmPhonecallMetadata.CreatedOn)),
                    ModifiedBy = context.Parse<long>(entity.Value(CrmPhonecallMetadata.ModifiedBy)),
                    ModifiedOn = context.Parse<DateTime>(entity.Value(CrmPhonecallMetadata.ModifiedOn)),
                    OwnerId = context.Parse<long?>(entity.Value(CrmPhonecallMetadata.OwnerId)),
                    Subject = context.Parse<string>(entity.Value(CrmPhonecallMetadata.Subject)),
                    Description = context.Parse<string>(entity.Value(CrmPhonecallMetadata.Description)),
                    Priority = context.Parse<int>(entity.Value(CrmPhonecallMetadata.PriorityCode)).Map(ToPriority),
                    Status = context.Parse<CrmPhonecallState>(entity.Value(CrmPhonecallMetadata.StateCode)).Map(ToStatus),
                    Purpose = context.Parse<int>(entity.Value(CrmPhonecallMetadata.Purpose)).Map(ToPurpose),
                    
                    // it might have empty schedule time
                    ScheduledOn = context.Parse<DateTime?>(entity.Value(CrmPhonecallMetadata.ScheduledStart))
                        ?? context.Parse<DateTime?>(entity.Value(CrmPhonecallMetadata.ActualStart))
                        ?? context.Parse<DateTime?>(entity.Value(CrmPhonecallMetadata.ActualEnd))
                        ?? context.Parse<DateTime>(entity.Value(CrmPhonecallMetadata.ModifiedOn)),

                    // requirement: привязанным объектом м.б. только клиент, фирма или сделка
                    RegardingObjects = regardingObjects
                        .Concat(recipients) // нужная информация может быть в получателях
                        .FilterByEntityName(ErmEntityName.Client, ErmEntityName.Firm, ErmEntityName.Deal)
                        .Select(x => x.ToReferenceWithin(context))
                        .Distinct() // it's safe as ActivityReference implements IEquatable<>
                        .ToList(),
                    
                    // requirement: получателем может быть только контакт
                    Recipients = recipients.Concat(regardingObjects)
                        .FilterByEntityName(ErmEntityName.Contact)
                        .Select(x => x.ToReferenceWithin(context))
                        .Distinct()
                        .ToList(),
                };

                return phonecall;
            }

            private static ErmPhonecallPriority ToPriority(int code)
            {
                switch (code)
                {
                    case 0:
                        return ErmPhonecallPriority.Low;
                    case 1:
                        return ErmPhonecallPriority.Average;
                    case 2:
                        return ErmPhonecallPriority.High;
                    default:
                        return ErmPhonecallPriority.NotSet;
                }
            }

            private static ErmPhonecallStatus ToStatus(CrmPhonecallState state)
            {
                switch (state)
                {
                    case CrmPhonecallState.Open:
                        return ErmPhonecallStatus.InProgress;
                    case CrmPhonecallState.Completed:
                        return ErmPhonecallStatus.Completed;
                    case CrmPhonecallState.Canceled:
                        return ErmPhonecallStatus.Canceled;
                    default:
                        return ErmPhonecallStatus.NotSet;
                }
            }

            private static ErmPhonecallPurpose ToPurpose(int code)
            {
                switch (code)
                {
                    case 1:
                        return ErmPhonecallPurpose.FirstCall;
                    case 3:
                        return ErmPhonecallPurpose.ProductPresentation;
                    case 4:
                        return ErmPhonecallPurpose.OpportunitiesPresentation;
                    case 5:
                        return ErmPhonecallPurpose.OfferApproval;
                    case 6:
                        return ErmPhonecallPurpose.DecisionApproval;
                    case 7:
                        return ErmPhonecallPurpose.Prolongation;
                    case 8:
                        return ErmPhonecallPurpose.Service;
                    case 9:
                        return ErmPhonecallPurpose.Upsale;
                    default:
                        return ErmPhonecallPurpose.NotSet;
                }
            }
        }

        internal enum PhonecallReferenceType
        {
            RegardingObject = 1,
            Recipient = 2,
        }

        #endregion
    }
}