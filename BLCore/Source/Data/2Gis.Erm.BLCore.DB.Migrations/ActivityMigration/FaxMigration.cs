using System;
using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Extensions;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration
{
    using CrmEntityName = Microsoft.Crm.SdkTypeProxy.EntityName;
    using CrmFaxMetadata = Metadata.Crm.Fax;
    using CrmFaxState = Microsoft.Crm.SdkTypeProxy.FaxState;
    using ErmEntityName = Metadata.Erm.EntityName;
    using ErmFaxPriority = Metadata.Erm.ActivityPriority;
    using ErmFaxStatus = Metadata.Erm.ActivityStatus;

    [Migration(23488, "Migrates the faxes as letters from CRM to ERM.", "s.pomadin")]
    public sealed class FaxMigration : LetterMigrationBase
    {
        internal override QueryExpression CreateQuery()
        {
            var query = new QueryExpression
                            {
                                EntityName = CrmEntityName.fax.ToString(),
                                ColumnSet = new ColumnSet(new[]
                                                              {
                                                                  CrmFaxMetadata.ActivityId,
                                                                  CrmFaxMetadata.CreatedBy,
                                                                  CrmFaxMetadata.CreatedOn,
                                                                  CrmFaxMetadata.ModifiedBy,
                                                                  CrmFaxMetadata.ModifiedOn,
                                                                  CrmFaxMetadata.OwnerId,
                                                                  CrmFaxMetadata.RegardingObjectId,
                                                                  CrmFaxMetadata.Subject,
                                                                  CrmFaxMetadata.Description,
                                                                  CrmFaxMetadata.ScheduledStart,
                                                                  CrmFaxMetadata.PriorityCode,
                                                                  CrmFaxMetadata.StateCode,
                                                                  CrmFaxMetadata.From,
                                                                  CrmFaxMetadata.To,
                                                              }),
                            };
            return query;
        }

        internal override Letter Create(IActivityMigrationContextExtended context, DynamicEntity entity)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (entity.Name != CrmEntityName.fax.ToString())
            {
                throw new ArgumentException("The specified entity is not a fax.", "entity");
            }

            var recipients = (entity.Value(CrmFaxMetadata.To) as DynamicEntity[]).EnumerateActivityReferences().ToList();

            var letter = new Letter
            {
                Id = context.NewIdentity(),
                ReplicationCode = context.Parse<Guid>(entity.Value(CrmFaxMetadata.ActivityId)),
                CreatedBy = context.Parse<long>(entity.Value(CrmFaxMetadata.CreatedBy)),
                CreatedOn = context.Parse<DateTime>(entity.Value(CrmFaxMetadata.CreatedOn)),
                ModifiedBy = context.Parse<long>(entity.Value(CrmFaxMetadata.ModifiedBy)),
                ModifiedOn = context.Parse<DateTime>(entity.Value(CrmFaxMetadata.ModifiedOn)),
                OwnerId = context.Parse<long?>(entity.Value(CrmFaxMetadata.OwnerId)),
                Subject = context.Parse<string>(entity.Value(CrmFaxMetadata.Subject)),
                Description = context.Parse<string>(entity.Value(CrmFaxMetadata.Description)),
                Priority = context.Parse<int>(entity.Value(CrmFaxMetadata.PriorityCode)).Map(ToPriority),
                Status = context.Parse<CrmFaxState>(entity.Value(CrmFaxMetadata.StateCode)).Map(ToStatus),
                
                // fax might have empty schedule time
                ScheduledOn = context.Parse<DateTime?>(entity.Value(CrmFaxMetadata.ScheduledStart))
                    ?? context.Parse<DateTime?>(entity.Value(CrmFaxMetadata.ActualStart))
                    ?? context.Parse<DateTime?>(entity.Value(CrmFaxMetadata.ActualEnd))
                    ?? context.Parse<DateTime>(entity.Value(CrmFaxMetadata.ModifiedOn)),

                // requirement: привязанным объектом м.б. только клиент, фирма или сделка
                RegardingObjects = new[] { entity.Value(CrmFaxMetadata.RegardingObjectId) as CrmReference }
                    .Concat(recipients) // нужная информация может быть в получателях
                    .FilterByEntityName(ErmEntityName.Client, ErmEntityName.Firm, ErmEntityName.Deal)
                    .Select(x => x.ToReferenceWithin(context))
                    .Distinct() // it's safe as ActivityReference implements IEquatable<>
                    .ToList(),
                
                // requirement: отправителем может быть только пользователь
                Senders = (entity.Value(CrmFaxMetadata.From) as DynamicEntity[]).EnumerateActivityReferences()
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

        private static ErmFaxPriority ToPriority(int code)
        {
            switch (code)
            {
                case 0:
                    return ErmFaxPriority.Low;
                case 1:
                    return ErmFaxPriority.Average;
                case 2:
                    return ErmFaxPriority.High;
                default:
                    return ErmFaxPriority.NotSet;
            }
        }

        private static ErmFaxStatus ToStatus(CrmFaxState state)
        {
            switch (state)
            {
                case CrmFaxState.Open:
                    return ErmFaxStatus.InProgress;
                case CrmFaxState.Completed:
                    return ErmFaxStatus.Completed;
                case CrmFaxState.Canceled:
                    return ErmFaxStatus.Canceled;
                default:
                    return ErmFaxStatus.NotSet;
            }
        }
    }
}