using System;
using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Extensions;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration
{
    using CrmEntityName = Microsoft.Crm.SdkTypeProxy.EntityName;
    using CrmEmailMetadata = Metadata.Crm.Email;
    using CrmEmailState = Microsoft.Crm.SdkTypeProxy.EmailState;
    using ErmEntityName = Metadata.Erm.EntityName;
    using ErmEmailPriority = Metadata.Erm.ActivityPriority;
    using ErmEmailStatus = Metadata.Erm.ActivityStatus;

    [Migration(23489, "Migrates the emails as letters from CRM to ERM.", "s.pomadin")]
    public sealed class EmailMigration : LetterMigration
    {
        internal override QueryExpression CreateQuery()
        {
            var query = new QueryExpression
            {
                EntityName = CrmEntityName.email.ToString(),
                ColumnSet = new ColumnSet(new[]
						{
							CrmEmailMetadata.ActivityId,
							CrmEmailMetadata.CreatedBy,
							CrmEmailMetadata.CreatedOn,
							CrmEmailMetadata.ModifiedBy,
							CrmEmailMetadata.ModifiedOn,
							CrmEmailMetadata.OwnerId,
							CrmEmailMetadata.RegardingObjectId,
							CrmEmailMetadata.Subject,
							CrmEmailMetadata.Description,
							CrmEmailMetadata.ScheduledStart,
							CrmEmailMetadata.ActualStart,
							CrmEmailMetadata.ActualEnd,
							CrmEmailMetadata.PriorityCode,
							CrmEmailMetadata.StateCode,
							CrmEmailMetadata.From,
							CrmEmailMetadata.To,
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
            if (entity.Name != CrmEntityName.email.ToString())
                throw new ArgumentException("The specified entity is not a fax.", "entity");

            var description = context.Parse<string>(entity.Value(CrmEmailMetadata.Description));
            if (description != null && description.Contains("<HTML>"))
            {
                throw new Exception("Requirement: Rich HTML document are not supported for email migration.");
            }

            var recipients = (entity.Value(CrmEmailMetadata.To) as DynamicEntity[]).EnumerateActivityReferences().ToList();

            var letter = new Letter
            {
                Id = context.NewIdentity(),
                ReplicationCode = context.Parse<Guid>(entity.Value(CrmEmailMetadata.ActivityId)),
                CreatedBy = context.Parse<long>(entity.Value(CrmEmailMetadata.CreatedBy)),
                CreatedOn = context.Parse<DateTime>(entity.Value(CrmEmailMetadata.CreatedOn)),
                ModifiedBy = context.Parse<long>(entity.Value(CrmEmailMetadata.ModifiedBy)),
                ModifiedOn = context.Parse<DateTime>(entity.Value(CrmEmailMetadata.ModifiedOn)),
                OwnerId = context.Parse<long?>(entity.Value(CrmEmailMetadata.OwnerId)),
                Subject = context.Parse<string>(entity.Value(CrmEmailMetadata.Subject)),
                Description = StripHtml(description),
                Priority = context.Parse<int>(entity.Value(CrmEmailMetadata.PriorityCode)).Map(ToPriority),
                Status = context.Parse<CrmEmailState>(entity.Value(CrmEmailMetadata.StateCode)).Map(ToStatus),
                // email might have empty schedule time
                ScheduledOn = context.Parse<DateTime?>(entity.Value(CrmEmailMetadata.ScheduledStart))
                    ?? context.Parse<DateTime?>(entity.Value(CrmEmailMetadata.ActualStart))
                    ?? context.Parse<DateTime?>(entity.Value(CrmEmailMetadata.ActualEnd))
                    ?? context.Parse<DateTime>(entity.Value(CrmEmailMetadata.ModifiedOn)),

                // requirement: привязанным объектом м.б. только клиент, фирма или сделка
                RegardingObjects = new[] { entity.Value(CrmEmailMetadata.RegardingObjectId) as CrmReference }
                    .Concat(recipients) // нужная информация может быть в получателях
                    .FilterByEntityName(ErmEntityName.Client, ErmEntityName.Firm, ErmEntityName.Deal)
                    .Select(x => x.ToReferenceWithin(context))
                    .Distinct() // it's safe as ActivityReference implements IEquatable<>
                    .ToList(),
                // requirement: отправителем может быть только пользователь
                Senders = (entity.Value(CrmEmailMetadata.From) as DynamicEntity[]).EnumerateActivityReferences()
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

        private static ErmEmailPriority ToPriority(int code)
        {
            switch (code)
            {
                case 0:
                    return ErmEmailPriority.Low;
                case 1:
                    return ErmEmailPriority.Average;
                case 2:
                    return ErmEmailPriority.High;
                default:
                    return ErmEmailPriority.NotSet;
            }
        }

        private static ErmEmailStatus ToStatus(CrmEmailState state)
        {
            switch (state)
            {
                case CrmEmailState.Open:
                    return ErmEmailStatus.InProgress;
                case CrmEmailState.Completed:
                    return ErmEmailStatus.Completed;
                case CrmEmailState.Canceled:
                    return ErmEmailStatus.Canceled;
                default:
                    return ErmEmailStatus.NotSet;
            }
        }

        private static string StripHtml(string html)
        {
            return string.IsNullOrWhiteSpace(html) ? html : Regex.Replace(html, @"<[^>]+>|&nbsp;", "");
        }
    }
}