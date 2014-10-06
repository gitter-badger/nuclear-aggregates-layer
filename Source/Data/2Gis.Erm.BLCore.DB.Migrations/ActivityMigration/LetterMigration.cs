using System;
using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Extensions;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration
{
    using CrmEntityName = Microsoft.Crm.SdkTypeProxy.EntityName;
    using CrmLetterMetadata = Metadata.Crm.Letter;
    using CrmLetterState = Microsoft.Crm.SdkTypeProxy.LetterState;
    using ErmEntityName = Metadata.Erm.EntityName;
    using ErmLetterPriority = Metadata.Erm.ActivityPriority;
    using ErmLetterStatus = Metadata.Erm.ActivityStatus;

    //[Migration(23487, "Migrates the letters from CRM to ERM.", "s.pomadin")]
    public class LetterMigration : LetterMigrationBase
    {
        internal override QueryExpression CreateQuery()
        {
            var query = new QueryExpression
            {
                EntityName = CrmEntityName.letter.ToString(),
                ColumnSet = new ColumnSet(new[]
						{
							CrmLetterMetadata.ActivityId,
							CrmLetterMetadata.CreatedBy,
							CrmLetterMetadata.CreatedOn,
							CrmLetterMetadata.ModifiedBy,
							CrmLetterMetadata.ModifiedOn,
							CrmLetterMetadata.OwnerId,
							CrmLetterMetadata.RegardingObjectId,
							CrmLetterMetadata.Subject,
							CrmLetterMetadata.Description,
							CrmLetterMetadata.ScheduledStart,
							CrmLetterMetadata.PriorityCode,
							CrmLetterMetadata.StateCode,
							CrmLetterMetadata.From,
							CrmLetterMetadata.To,
						}),
            };
            return query;
        }

        internal override Letter Create(IActivityMigrationContextExtended context, DynamicEntity entity)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (entity.Name != CrmEntityName.letter.ToString())
                throw new ArgumentException("The specified entity is not a letter.", "entity");

            var recipients = (entity.Value(CrmLetterMetadata.To) as DynamicEntity[]).EnumerateActivityReferences().ToList();

            var letter = new Letter
            {
                Id = context.NewIdentity(),
                ReplicationCode = context.Parse<Guid>(entity.Value(CrmLetterMetadata.ActivityId)),
                CreatedBy = context.Parse<long>(entity.Value(CrmLetterMetadata.CreatedBy)),
                CreatedOn = context.Parse<DateTime>(entity.Value(CrmLetterMetadata.CreatedOn)),
                ModifiedBy = context.Parse<long>(entity.Value(CrmLetterMetadata.ModifiedBy)),
                ModifiedOn = context.Parse<DateTime>(entity.Value(CrmLetterMetadata.ModifiedOn)),
                OwnerId = context.Parse<long?>(entity.Value(CrmLetterMetadata.OwnerId)),
                Subject = context.Parse<string>(entity.Value(CrmLetterMetadata.Subject)),
                Description = context.Parse<string>(entity.Value(CrmLetterMetadata.Description)),
                Priority = context.Parse<int>(entity.Value(CrmLetterMetadata.PriorityCode)).Map(ToPriority),
                Status = context.Parse<CrmLetterState>(entity.Value(CrmLetterMetadata.StateCode)).Map(ToStatus),
                // letter might have empty schedule time
                ScheduledOn = context.Parse<DateTime?>(entity.Value(CrmLetterMetadata.ScheduledStart))
                    ?? context.Parse<DateTime?>(entity.Value(CrmLetterMetadata.ActualStart))
                    ?? context.Parse<DateTime?>(entity.Value(CrmLetterMetadata.ActualEnd))
                    ?? context.Parse<DateTime>(entity.Value(CrmLetterMetadata.ModifiedOn)),

                // requirement: привязанным объектом м.б. только клиент, фирма или сделка
                RegardingObjects = new[] { entity.Value(CrmLetterMetadata.RegardingObjectId) as CrmReference }
                    .Concat(recipients) // нужная информация может быть в получателях
                    .FilterByEntityName(ErmEntityName.Client, ErmEntityName.Firm, ErmEntityName.Deal)
                    .Select(x => x.ToReferenceWithin(context))
                    .Distinct() // it's safe as ActivityReference implements IEquatable<>
                    .ToList(),
                // requirement: отправителем может быть только пользователь
                Senders = (entity.Value(CrmLetterMetadata.From) as DynamicEntity[]).EnumerateActivityReferences()
                    .FilterByEntityName(ErmEntityName.User)
                    .Select(x => x.ToReferenceWithin(context))
                    .ToList(),
                // requirement: получателем может быть только контакт
                Recipients = recipients
                    .FilterByEntityName(ErmEntityName.Contact)
                    .Select(x => x.ToReferenceWithin(context))
                    .ToList(),
            };

            return letter;
        }

        private static ErmLetterPriority ToPriority(int code)
        {
            switch (code)
            {
                case 0:
                    return ErmLetterPriority.Low;
                case 1:
                    return ErmLetterPriority.Average;
                case 2:
                    return ErmLetterPriority.High;
                default:
                    return ErmLetterPriority.NotSet;
            }
        }

        private static ErmLetterStatus ToStatus(CrmLetterState state)
        {
            switch (state)
            {
                case CrmLetterState.Open:
                    return ErmLetterStatus.InProgress;
                case CrmLetterState.Completed:
                    return ErmLetterStatus.Completed;
                case CrmLetterState.Canceled:
                    return ErmLetterStatus.Canceled;
                default:
                    return ErmLetterStatus.NotSet;
            }
        }
    }
}