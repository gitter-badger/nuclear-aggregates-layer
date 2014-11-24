using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Extensions;

using Microsoft.Crm.Sdk;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration
{
    using ErmActivityPriority = Metadata.Erm.ActivityPriority;
    using ErmActivityStatus = Metadata.Erm.ActivityStatus;

    public abstract class LetterMigrationBase : ActivityMigration<LetterMigrationBase.Letter>
    {
        private const string InsertEntityTemplate = @"
INSERT INTO [Activity].[LetterBase]
	([Id],[ReplicationCode],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn],[IsActive],[IsDeleted],[OwnerCode],
	 [Subject],[Description],[ScheduledOn],[Priority],[Status])
	VALUES ({0}, {1}, {2}, {3}, {4}, {5}, 1, 0, {6},
			{7}, {8}, {9}, {10}, {11})";
        private const string InsertReferenceTemplate = @"
INSERT INTO [Activity].[LetterReferences]
           ([LetterId],[Reference],[ReferencedType],[ReferencedObjectId])
Values ({0}, {1}, {2}, {3})";

        internal sealed override Letter ParseActivity(IActivityMigrationContextExtended context, DynamicEntity entity)
        {
            return Create(context, entity);
        }

        internal abstract Letter Create(IActivityMigrationContextExtended context, DynamicEntity entity);

        internal sealed override string BuildSql(Letter letter)
        {
            var sb = new StringBuilder();

            sb.AppendLine(QueryBuilder.Format(InsertEntityTemplate,
                letter.Id, letter.ReplicationCode,
                letter.CreatedBy, letter.CreatedOn, letter.ModifiedBy, letter.ModifiedOn, letter.OwnerId,
                letter.Subject, letter.Description, letter.ScheduledOn, letter.Priority, letter.Status));

            // regarding object
            foreach (var regardingObject in letter.RegardingObjects)
            {
                BuildSqlStatement<long>(InsertReferenceTemplate, letter.Id, LetterReferenceType.RegardingObject, regardingObject.EntityName, regardingObject.EntityId).DoIfNotNull(x => sb.AppendLine(x));
            }
            // senders
            foreach (var receiver in letter.Senders)
            {
                BuildSqlStatement<long>(InsertReferenceTemplate, letter.Id, LetterReferenceType.Sender, receiver.EntityName, receiver.EntityId).DoIfNotNull(x => sb.AppendLine(x));
            }
            // recipients
            foreach (var receiver in letter.Recipients)
            {
                BuildSqlStatement<long>(InsertReferenceTemplate, letter.Id, LetterReferenceType.Recipient, receiver.EntityName, receiver.EntityId).DoIfNotNull(x => sb.AppendLine(x));
            }

            return sb.ToString();
        }

        #region Letter

        [DebuggerDisplay("Letter: {Subject}, {Status}")]
        public sealed class Letter
        {
            public long Id { get; set; }
            public long CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }
            public long ModifiedBy { get; set; }
            public DateTime ModifiedOn { get; set; }
            public Guid ReplicationCode { get; set; }

            public long? OwnerId { get; set; }
            public IEnumerable<ActivityReference> RegardingObjects { get; set; }
            public IEnumerable<ActivityReference> Senders { get; set; }
            public IEnumerable<ActivityReference> Recipients { get; set; }

            public string Subject { get; set; }
            public string Description { get; set; }
            public DateTime? ScheduledOn { get; set; }
            public ErmActivityPriority Priority { get; set; }
            public ErmActivityStatus Status { get; set; }
        }

        internal enum LetterReferenceType
        {
            RegardingObject = 1,
            Sender = 2,
            Recipient = 3,
        }

        #endregion
    }
}