using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class NotificationEmails :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity,
        IDeactivatableEntity,
        IStateTrackingEntity
    {
        public NotificationEmails()
        {
            NotificationEmailsAttachments = new HashSet<NotificationEmailsAttachments>();
            NotificationEmailsCc = new HashSet<NotificationEmailsCc>();
            NotificationEmailsTo = new HashSet<NotificationEmailsTo>();
            NotificationProcessings = new HashSet<NotificationProcessings>();
        }

        public long Id { get; set; }
        public long? SenderId { get; set; }
        public string Subject { get; set; }
        public string SubjectEncoding { get; set; }
        public string Body { get; set; }
        public string BodyEncoding { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public string Priority { get; set; }
        public int? MaxAttemptsCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public bool IsBodyHtml { get; set; }

        public NotificationAddresses Sender { get; set; }
        public ICollection<NotificationEmailsAttachments> NotificationEmailsAttachments { get; set; }
        public ICollection<NotificationEmailsCc> NotificationEmailsCc { get; set; }
        public ICollection<NotificationEmailsTo> NotificationEmailsTo { get; set; }
        public ICollection<NotificationProcessings> NotificationProcessings { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var entityKey = obj as IEntityKey;
            if (entityKey != null)
            {
                return Id == entityKey.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}